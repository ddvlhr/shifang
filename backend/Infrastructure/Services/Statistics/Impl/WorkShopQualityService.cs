using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Dtos.WorkShopQuality;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Services.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Services.Statistics.Impl;

public class WorkShopQualityService : IWorkShopQualityService
{
    private readonly IRepository<InspectionReportAppearance> _iraRepo;
    private readonly IInspectionReportAppearanceService _iraService;
    private readonly IRepository<InspectionReport> _irRepo;
    private readonly IRepository<ProductReportAppearance> _praRepo;
    private readonly IProductReportAppearanceService _praService;
    private readonly IRepository<ProductReport> _prRepo;
    private readonly IRepository<PhysicalReportAppearance> _psaRepo;
    private readonly IRepository<PhysicalReport> _psRepo;
    private readonly Settings _settings;
    private readonly IRepository<WorkShopQualityPointRule> _wsqprRepo;

    public WorkShopQualityService(IRepository<InspectionReport> irRepo,
        IRepository<ProductReport> prRepo,
        IRepository<PhysicalReport> psRepo,
        IRepository<PhysicalReportAppearance> psaRepo,
        IRepository<InspectionReportAppearance> iraRepo,
        IRepository<ProductReportAppearance> praRepo,
        IInspectionReportAppearanceService iraService,
        IProductReportAppearanceService praService,
        IRepository<WorkShopQualityPointRule> wsqprRepo,
        IOptionsSnapshot<Settings> settings)
    {
        _irRepo = irRepo;
        _prRepo = prRepo;
        _psRepo = psRepo;
        _psaRepo = psaRepo;
        _iraRepo = iraRepo;
        _praRepo = praRepo;
        _iraService = iraService;
        _praService = praService;
        _wsqprRepo = wsqprRepo;
        _settings = settings.Value;
    }

    public IEnumerable<WorkShopQualityInfoDto> GetInfo(WorkShopQualityQueryInfoDto dto)
    {
        var data = getQualityInfo(dto);

        return data;
    }

    public MemoryStream Download(WorkShopQualityQueryInfoDto dto)
    {
        var data = getQualityInfo(dto);

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("车间质量统计");
        ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        ws.Cells.Style.WrapText = true;
        ws.Row(1).Height = 30;
        ws.Row(1).Style.Font.Size = 14;
        ws.Row(1).Style.Font.Bold = true;
        ws.Cells[1, 1].Value = "车间名称";
        ws.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 2].Value = "班次/牌号类型名称";
        ws.Cells[1, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 3].Value = "机台名称";
        ws.Cells[1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 4].Value = "质量巡检得分";
        ws.Cells[1, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 5].Value = "质量物检得分";
        ws.Cells[1, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 6].Value = "质量成品得分";
        ws.Cells[1, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 7].Value = "质量综合得分";
        ws.Cells[1, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 8].Value = "班质量平均得分";
        ws.Cells[1, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 9].Value = "产品质量平均得分";
        ws.Cells[1, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Column(1).Width = 20;
        ws.Column(2).Width = 30;
        ws.Column(3).Width = 20;
        ws.Column(4).Width = 20;
        ws.Column(5).Width = 20;
        ws.Column(6).Width = 20;
        ws.Column(7).Width = 20;
        ws.Column(8).Width = 20;
        ws.Column(9).Width = 30;

        var row = 2;
        var index = 1;
        foreach (var item in data)
        {
            ws.Row(row).Height = 25;
            ws.Cells[row, 1].Value = item.WorkShopName;
            ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 2].Value = item.TurnName;
            ws.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 3].Value = item.MachineModelName;
            ws.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 4].Value = item.InspectionQualityValueStr;
            ws.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 5].Value = item.PhysicalQualityValueStr;
            ws.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 6].Value = item.ProductQualityValueStr;
            ws.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 7].Value = item.MachineQualityValueAverage;
            ws.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 8].Value = item.TurnQualityValueAverage;
            ws.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 9].Value = item.WorkShopQualityValueAverage;
            ws.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            if (item.TurnNameRowCount > 0)
            {
                ws.Cells[row, 2, row + item.TurnNameRowCount - 1, 2].Merge = true;
                ws.Cells[row, 2, row + item.TurnNameRowCount - 1, 2].Style.Border
                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                ws.Cells[row, 8, row + item.TurnNameRowCount - 1, 8].Merge = true;
                ws.Cells[row, 8, row + item.TurnNameRowCount - 1, 8].Style.Border
                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }

            if (item.WorkShopNameRowCount > 0)
            {
                ws.Cells[row, 1, row + item.WorkShopNameRowCount - 1, 1].Merge = true;
                ws.Cells[row, 1, row + item.WorkShopNameRowCount - 1, 1].Style.Border
                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);

                ws.Cells[row, 9, row + item.WorkShopNameRowCount - 1, 9].Merge = true;
                ws.Cells[row, 9, row + item.WorkShopNameRowCount - 1, 9].Style.Border
                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
            }

            row++;
        }

        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }

    private IEnumerable<WorkShopQualityInfoDto> getQualityInfo(WorkShopQualityQueryInfoDto dto)
    {
        var workShopId = 0;
        var specificationTypeId = 0;
        var list = new List<WorkShopQualityInfoDto>();

        if (!string.IsNullOrEmpty(dto.WorkShopId)) workShopId = int.Parse(dto.WorkShopId);

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId)) specificationTypeId = int.Parse(dto.SpecificationTypeId);

        var rules = _wsqprRepo.All().ToList();
        var tempList = new List<WorkShopQualityInfoDto>();
        tempList.AddRange(getInspection(workShopId, specificationTypeId, dto.BeginDate, dto.EndDate));
        tempList.AddRange(getProduct(workShopId, specificationTypeId, dto.BeginDate, dto.EndDate));
        tempList.AddRange(getPhysical(workShopId, specificationTypeId, dto.BeginDate, dto.EndDate));

        if (tempList.Count > 0)
        {
            List<IGrouping<string, WorkShopQualityInfoDto>> group;
            if (workShopId == _settings.BoxMakingWorkShopId)
                group = tempList.GroupBy(c => c.SpecificationTypeName).ToList();
            else
                group = tempList.GroupBy(c => c.TurnName).ToList();
            var workShopQualityList = new Dictionary<int, double>();
            foreach (var g in group)
            {
                var groupFirst = g.FirstOrDefault();
                if (groupFirst == null)
                    continue;
                var rule = rules.FirstOrDefault(c =>
                    c.WorkShopId == workShopId && c.SpecificationTypeId == groupFirst.SpecificationTypeId);
                if (rule == null)
                    continue;
                var inspectionPercent = Convert.ToDouble(rule.InspectionAllPercent) / 100;
                var productPercent = Convert.ToDouble(rule.ProductAllPercent) / 100;
                var physicalPercent = Convert.ToDouble(rule.PhysicalAllPercent) / 100;

                var machineGroups = g.GroupBy(c => c.MachineModelName).ToList();
                var machineModelList = new List<WorkShopQualityInfoDto>();
                var machineModelQualityList = new List<double>();
                foreach (var machineGroup in machineGroups)
                {
                    var first = machineGroup.FirstOrDefault();
                    if (first == null)
                        continue;
                    var inspectionValueList = machineGroup.Where(c => c.InspectionQualityValue > 0)
                        .Select(c => c.InspectionQualityValue).ToList();
                    var productValueList = machineGroup.Where(c => c.ProductQualityValue > 0)
                        .Select(c => c.ProductQualityValue).ToList();
                    var physicalValueList = machineGroup.Where(c => c.PhysicalQualityValue > 0).Select(c => c
                        .PhysicalQualityValue).ToList();
                    var inspectionAverage = inspectionValueList.Count == 0 ? 0 : inspectionValueList.Average();
                    var productAverage = productValueList.Count == 0 ? 0 : productValueList.Average();
                    var physicalAverage = physicalValueList.Count == 0 ? 0 : physicalValueList.Average();
                    double machineModelQuality = 0;
                    double mole = 0;
                    double deno = 0;
                    if (inspectionAverage > 0 && productAverage > 0)
                    {
                        var inspectionValue = inspectionAverage * inspectionPercent;
                        mole += inspectionValue;
                        deno = inspectionPercent;
                    }

                    if (productAverage > 0)
                    {
                        var productValue = productAverage * productPercent;
                        mole += productValue;
                        deno += productPercent;
                    }

                    if (physicalAverage > 0)
                    {
                        var physicalValue = physicalAverage * physicalPercent;
                        mole += physicalValue;
                        deno += physicalPercent;
                    }

                    machineModelQuality = mole / deno;
                    machineModelQualityList.Add(machineModelQuality);

                    if (workShopId == _settings.BoxMakingWorkShopId &&
                        _settings.RecycleBoxTypeId.Contains(first.SpecificationTypeId))
                    {
                        machineModelQuality = productAverage;
                        machineModelQualityList.Add(productAverage);
                    }

                    var turnName = first.TurnName;
                    if (workShopId == _settings.BoxMakingWorkShopId) turnName = first.SpecificationTypeName;

                    var inspectionQualityAverage =
                        inspectionAverage.Equals(0) ? "" : inspectionAverage.ToString("F2");
                    var productQualityAverage = productAverage.Equals(0) ? "" : productAverage.ToString("F2");
                    var physicalQualityAverage = physicalAverage.Equals(0) ? "" : physicalAverage.ToString("F2");
                    var item = new WorkShopQualityInfoDto
                    {
                        WorkShopName = first.WorkShopName,
                        SpecificationTypeName = first.SpecificationTypeName,
                        MachineModelName = first.MachineModelName,
                        TurnName = turnName,
                        InspectionQualityValueStr = inspectionQualityAverage,
                        ProductQualityValueStr = productQualityAverage,
                        PhysicalQualityValueStr = physicalQualityAverage,
                        MachineQualityValueAverage = machineModelQuality.ToString("F2")
                    };
                    machineModelList.Add(item);
                }

                var machineModelFirst = machineModelList.FirstOrDefault();
                if (machineModelFirst == null)
                    continue;
                if (machineModelQualityList.Count > 0)
                {
                    var machineModelQualityAverage = machineModelQualityList.Average();
                    workShopQualityList.Add(
                        workShopId == _settings.BoxMakingWorkShopId
                            ? groupFirst.SpecificationTypeId
                            : groupFirst.TurnId, machineModelQualityAverage);

                    machineModelFirst.TurnQualityValueAverage =
                        machineModelQualityAverage.ToString("F2");
                }


                machineModelFirst.TurnNameRowCount = machineModelList.Count;

                list.AddRange(machineModelList);
            }

            var listFirst = list.FirstOrDefault();
            listFirst.WorkShopNameRowCount = list.Count;
            double workShopQualityAverage = 0;
            if (workShopId == _settings.BoxMakingWorkShopId)
            {
                var receiveBox = workShopQualityList.Where(c => _settings.RecycleBoxTypeId.Contains(c.Key))
                    .Select(c => c.Value).ToList();
                var otherBox = workShopQualityList.Where(c => !_settings.RecycleBoxTypeId.Contains(c.Key))
                    .Select(c => c.Value).ToList();
                double receiveBoxAverage = 0;
                if (receiveBox.Count > 0)
                    receiveBoxAverage = receiveBox.Average();
                double otherBoxAverage = 0;
                if (otherBox.Count > 0)
                    otherBoxAverage = otherBox.Average();
                workShopQualityAverage = receiveBoxAverage * 0.4 + otherBoxAverage * 0.6;
            }
            else
            {
                if (workShopQualityList.Count > 0)
                {
                    var values = workShopQualityList.Select(c => c.Value).ToList();
                    workShopQualityAverage = values.Average();
                }
            }


            listFirst.WorkShopQualityValueAverage = workShopQualityAverage.ToString("F2");
        }


        return list;
    }

    private IEnumerable<WorkShopQualityInfoDto> getInspection(int workShopId, int specificationTypeId, string beginDate,
        string endDate)
    {
        var list = new List<WorkShopQualityInfoDto>();
        var data = _irRepo.All().AsNoTracking();
        if (workShopId > 0) data = data.Where(c => c.Group.MachineModel.WorkShopId == workShopId);

        if (specificationTypeId > 0)
            data = data.Where(c => c.Group.Specification.SpecificationTypeId == specificationTypeId);

        if (!string.IsNullOrEmpty(beginDate) && !string.IsNullOrEmpty(endDate))
        {
            var begin = Convert.ToDateTime(beginDate);
            var end = Convert.ToDateTime(endDate);
            data = data.Where(c => c.Group.BeginTime >= begin &&
                                   c.Group.EndTime <= end);
        }

        var tempList = data.Include(c => c.Group)
            .ThenInclude(c => c.MachineModel).ThenInclude(c => c.WorkShop)
            .Include(c => c.Group).ThenInclude(c => c.Specification).ThenInclude(c => c.SpecificationType)
            .Include(c => c.Group).ThenInclude(c => c.Turn)
            .Include(c => c.Group).ThenInclude(c => c.MachineModel).ToList();

        var reportIds = tempList.Select(c => c.Id).ToList();
        var appearances = _iraRepo.All().Where(c => reportIds.Contains(c.ReportId)).Select(c => new
        {
            c.ReportId,
            c.SubScore,
            c.Frequency
        }).ToList();
        foreach (var item in tempList)
        {
            var score = appearances.Where(c => c.ReportId == item.Id).Sum(c => c.SubScore * c.Frequency);
            var temp = new WorkShopQualityInfoDto
            {
                WorkShopId = item.Group.MachineModel.WorkShopId,
                WorkShopName = item.Group.MachineModel.WorkShop.Name,
                MachineModelName = item.Group.MachineModel.Name,
                // TurnId = item.Group.TurnId,
                TurnName = item.Group.Turn.Name,
                SpecificationTypeId = item.Group.Specification.SpecificationTypeId,
                SpecificationTypeName = item.Group.Specification.SpecificationType.Name,
                InspectionQualityValue = 100 - item.PhyRetDeduction - score
            };

            list.Add(temp);
        }

        return list;
    }

    private IEnumerable<WorkShopQualityInfoDto> getProduct(int workShopId, int specificationTypeId, string beginDate,
        string endDate)
    {
        var list = new List<WorkShopQualityInfoDto>();
        var data = _prRepo.All().AsNoTracking();
        if (workShopId > 0) data = data.Where(c => c.Group.MachineModel.WorkShopId == workShopId);

        if (specificationTypeId > 0)
            data = data.Where(c => c.Group.Specification.SpecificationTypeId == specificationTypeId);

        if (!string.IsNullOrEmpty(beginDate) && !string.IsNullOrEmpty(endDate))
        {
            var begin = Convert.ToDateTime(beginDate);
            var end = Convert.ToDateTime(endDate);
            data = data.Where(c => c.Group.BeginTime >= begin && c.Group.EndTime <= end);
        }

        var tempList = data.Include(c => c.Group)
            .ThenInclude(c => c.MachineModel).ThenInclude(c => c.WorkShop)
            .Include(c => c.Group).ThenInclude(c => c.Specification).ThenInclude(c => c.SpecificationType)
            .Include(c => c.Group).ThenInclude(c => c.Turn)
            .Include(c => c.Group).ThenInclude(c => c.MachineModel).ToList();
        var reportIds = tempList.Select(c => c.Id).ToList();
        var appearances = _praRepo.All().Where(c => reportIds.Contains(c.ReportId)).Select(c => new
        {
            c.ReportId,
            c.SubScore,
            c.Frequency
        }).ToList();
        foreach (var item in tempList)
        {
            var score = appearances.Where(c => c.ReportId == item.Id).Sum(c => c.SubScore * c.Frequency);
            var temp = new WorkShopQualityInfoDto
            {
                WorkShopId = item.Group.MachineModel.WorkShopId,
                WorkShopName = item.Group.MachineModel.WorkShop.Name,
                MachineModelName = item.Group.MachineModel.Name,
                TurnName = item.Group.Turn.Name,
                // TurnId = item.Group.TurnId,
                SpecificationTypeId = item.Group.Specification.SpecificationTypeId,
                SpecificationTypeName = item.Group.Specification.SpecificationType.Name,
                ProductQualityValue = 100 - item.PhyRetDeduction - score
            };

            list.Add(temp);
        }

        return list;
    }

    private IEnumerable<WorkShopQualityInfoDto> getPhysical(int workShopId, int specificationTypeId, string
        beginDate, string endDate)
    {
        var list = new List<WorkShopQualityInfoDto>();
        var data = _psRepo.All().AsNoTracking();
        if (workShopId > 0) data = data.Where(c => c.Group.MachineModel.WorkShopId == workShopId);

        if (specificationTypeId > 0)
            data = data.Where(c => c.Group.Specification.SpecificationTypeId == specificationTypeId);

        if (!string.IsNullOrEmpty(beginDate) && !string.IsNullOrEmpty(endDate))
        {
            var begin = Convert.ToDateTime(beginDate);
            var end = Convert.ToDateTime(endDate);
            data = data.Where(c => c.Group.BeginTime >= begin && c.Group.EndTime <= end);
        }

        var tempList = data.Include(c => c.Group)
            .ThenInclude(c => c.MachineModel).ThenInclude(c => c.WorkShop)
            .Include(c => c.Group).ThenInclude(c => c.Specification).ThenInclude(c => c.SpecificationType)
            .Include(c => c.Group).ThenInclude(c => c.Turn)
            .Include(c => c.Group).ThenInclude(c => c.MachineModel).ToList();
        var reportIds = tempList.Select(c => c.Id).ToList();
        var appearances = _psaRepo.All().Where(c => reportIds.Contains(c.ReportId)).Select(c => new
        {
            c.ReportId,
            c.SubScore,
            c.Frequency
        }).ToList();
        foreach (var item in tempList)
        {
            var score = appearances.Where(c => c.ReportId == item.Id).Sum(c => c.SubScore * c.Frequency);
            var temp = new WorkShopQualityInfoDto
            {
                WorkShopId = item.Group.MachineModel.WorkShopId,
                WorkShopName = item.Group.MachineModel.WorkShop.Name,
                MachineModelName = item.Group.MachineModel.Name,
                TurnName = item.Group.Turn.Name,
                // TurnId = item.Group.TurnId,
                SpecificationTypeId = item.Group.Specification.SpecificationTypeId,
                SpecificationTypeName = item.Group.Specification.SpecificationType.Name,
                PhysicalQualityValue = 100 - item.PhyRetDeduction - score
            };

            list.Add(temp);
        }

        return list;
    }
}