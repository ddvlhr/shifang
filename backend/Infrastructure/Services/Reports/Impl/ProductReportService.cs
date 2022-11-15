using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Dtos.Specification;
using Core.Dtos.MetricalData;
using Core.Dtos.Report;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Infrastructure.Helper;
using Infrastructure.Services.MetricalData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Services.Reports.Impl;

public class ProductReportService : IProductReportService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<MeasureTypeIndicators> _mtiRepo;
    private readonly IRepository<ProductReportAppearance> _praRepo;
    private readonly IRepository<ProductReport> _prRepo;
    private readonly IRepository<Role> _rRepo;
    private readonly IServiceProvider _services;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;
    private readonly IRepository<Turn> _tRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<WaterRecord> _wrRepo;

    public ProductReportService(IRepository<ProductReport> prRepo, IRepository<Group> gRepo,
        IRepository<Core.Entities.Specification> spRepo, IRepository<MeasureTypeIndicators> mtiRepo,
        IUnitOfWork uow, IRepository<ProductReportAppearance> praRepo, IRepository<Data> dRepo,
        IRepository<Turn> tRepo, IServiceProvider services,
        IRepository<WaterRecord> wrRepo, IRepository<Core.Entities.Indicator> iRepo,
        IOptionsSnapshot<Settings> settings,
        IHttpContextAccessor accessor, IRepository<Role> rRepo)
    {
        _prRepo = prRepo;
        _gRepo = gRepo;
        _spRepo = spRepo;
        _mtiRepo = mtiRepo;
        _uow = uow;
        _praRepo = praRepo;
        _dRepo = dRepo;
        _tRepo = tRepo;
        _services = services;
        _wrRepo = wrRepo;
        _iRepo = iRepo;
        _accessor = accessor;
        _rRepo = rRepo;
        _settings = settings.Value;
    }

    public bool Add(AddReportDto dto)
    {
        var report = new ProductReport();
        var isEdit = false;
        if (_prRepo.All().Any(c => c.GroupId == dto.GroupId))
        {
            isEdit = true;
            report = _prRepo.All()
                .FirstOrDefault(c => c.GroupId == dto.GroupId);
            report.Water = dto.Water;
            report.Humidity = dto.Humidity;
            report.Temperature = dto.Temperature;
            report.PhyRet = (ReportRet)dto.PhyRet;
            report.PhyRetDes = dto.PhyRetDes;
            report.Remark = dto.Remark;
            report.FinalRet = (ReportRet)dto.FinalRet;
        }
        else
        {
            report.GroupId = dto.GroupId;
            report.Water = dto.Water;
            report.Humidity = dto.Humidity;
            report.Temperature = dto.Temperature;
            report.PhyRet = (ReportRet)dto.PhyRet;
            report.PhyRetDes = dto.PhyRetDes;
            report.Remark = dto.Remark;
            report.FinalRet = (ReportRet)dto.FinalRet;
        }

        if (isEdit)
            _prRepo.Update(report);
        else
            _prRepo.Add(report);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;

        return result;
    }

    public bool Add(MetricalDataEditDataDto dto)
    {
        var report = new ProductReport
        {
            GroupId = dto.GroupId
        };
        var isEdit = false;
        if (_prRepo.All().Any(c => c.GroupId == dto.GroupId))
        {
            isEdit = true;
            report = _prRepo.All().FirstOrDefault(c => c.GroupId == dto.GroupId);
        }

        var group = _gRepo.Get(dto.GroupId);
        var specification = _spRepo.Get(group.SpecificationId);
        var typeId = group.MeasureTypeId;
        var phyRetDes = new List<string>();
        var phyRetDeduction = 0;
        var phyRetCount = 0;
        var singleRules = new List<Rule>();
        var meanRules = new List<Rule>();
        var sdRules = new List<Rule>();
        var cpkRules = new List<Rule>();
        var cvRules = new List<Rule>();
        if (!string.IsNullOrEmpty(specification.SingleRules))
            singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);

        if (!string.IsNullOrEmpty(specification.MeanRules))
            meanRules = JsonConvert.DeserializeObject<List<Rule>>(specification.MeanRules);

        if (!string.IsNullOrEmpty(specification.SdRules))
            sdRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SdRules);

        if (!string.IsNullOrEmpty(specification.CpkRules))
            cpkRules = JsonConvert.DeserializeObject<List<Rule>>(specification.CpkRules);

        if (!string.IsNullOrEmpty(specification.CvRules))
            cvRules = JsonConvert.DeserializeObject<List<Rule>>(specification.CvRules);

        var indicatorValues = _mtiRepo.All().AsNoTracking();
        var data = (JArray)JsonConvert.DeserializeObject(dto.DataInfo);
        var appearances = new List<ProductReportAppearance>();
        var listDics = new Dictionary<int, List<double>>();
        if (singleRules != null)
            foreach (var rule in singleRules)
            {
                // 获取物理指标分值管理中的对应指标Id的数据
                var indicatorValue = indicatorValues
                    .FirstOrDefault(c => c.IndicatorId == rule.Id &&
                                         c.MeasureTypeId == typeId &&
                                         c.SpecificationTypeId == specification.SpecificationTypeId);
                if (indicatorValue == null) continue;

                var standard = Convert.ToDouble(rule.Standard);
                var upper = Convert.ToDouble(rule.Upper);
                var lower = Convert.ToDouble(rule.Lower);
                // 用于暂存当前指标的测量数据
                var tempDataList = new List<double>();
                var currentAppCount = 0;

                foreach (var item in data)
                    // 当前指标不存在数据时跳过
                    if (item[$"{rule.Id}"] != null)
                    {
                        // 获取当前指标的测量值
                        var value = item[$"{rule.Id}"].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            // 指标标准为0时为外观指标
                            if (standard.Equals(0))
                            {
                                var frequency = Convert.ToInt32(value);
                                currentAppCount += frequency;
                            }
                            else
                            {
                                if (rule.Name == "水分")
                                {
                                    var water = Convert.ToDouble(value);
                                    var upLimit = standard + upper;
                                    var downLimit = standard - lower;
                                    double distance = 0;
                                    if (water > upLimit)
                                        distance = water - upLimit;
                                    else if (water < downLimit) distance = downLimit - water;

                                    var subScore = distance / 0.1 * indicatorValue.Deduction;
                                    if (subScore > indicatorValue.Points && indicatorValue.Points > 0)
                                    {
                                        phyRetDes.Add(rule.Name);
                                        phyRetDeduction += indicatorValue.Points;
                                    }
                                    else
                                    {
                                        phyRetDeduction += Convert.ToInt32(subScore);
                                    }
                                }
                                else
                                {
                                    // 将物理指标测量数据存入暂存list
                                    tempDataList.Add(Convert.ToDouble(value));
                                }
                            }
                        }
                    }

                if (standard.Equals(0))
                    if (currentAppCount > 0)
                    {
                        var appDeduction = 1;
                        if (indicatorValue != null) appDeduction = indicatorValue.Deduction;

                        if (indicatorValue.UnQualifiedOperator == UnQualifiedOperator.MoreThan)
                        {
                            if (currentAppCount > indicatorValue.UnQualifiedCount) phyRetCount++;
                        }
                        else if (indicatorValue.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual)
                        {
                            if (currentAppCount >= indicatorValue.UnQualifiedCount) phyRetCount++;
                        }

                        var subScore = currentAppCount * appDeduction;
                        var appearance = new ProductReportAppearance
                        {
                            IndicatorId = rule.Id,
                            Frequency = currentAppCount,
                            SubScore = subScore > indicatorValue.Points && indicatorValue.Points > 0
                                ? indicatorValue.Points
                                : subScore
                        };
                        appearances.Add(appearance);
                    }

                var statistic = tempDataList.toStatistic(standard, upper, lower);
                var cnt = statistic.HighCnt + statistic.LowCnt;
                var deduction = cnt * indicatorValue.Deduction;
                var sdRule = sdRules.FirstOrDefault(c => c.Id == rule.Id);
                var meanRule = meanRules.FirstOrDefault(c => c.Id == rule.Id);
                var cpkRule = cpkRules.FirstOrDefault(c => c.Id == rule.Id);
                var cvRule = cvRules.FirstOrDefault(c => c.Id == rule.Id);

                #region 计算 sd 值扣分

                if (sdRule != null)
                {
                    var tempSdList = new List<double> { statistic.Sd };
                    var sdSta = tempSdList.toStatistic(double.Parse(sdRule.Standard), double.Parse(sdRule.Upper),
                        double.Parse(sdRule.Lower));
                    var sdDeduction = (sdSta.HighCnt + sdSta.LowCnt) * indicatorValue.SdDeduction;
                    if (sdDeduction > 0)
                    {
                        phyRetDes.Add($"{rule.Name}SD值");
                        if (sdDeduction > indicatorValue.SdPoints && indicatorValue.SdPoints > 0)
                            phyRetDeduction += indicatorValue.SdPoints;
                        else
                            phyRetDeduction += sdDeduction;
                    }
                }

                #endregion

                #region 计算 mean 值扣分

                if (meanRule != null)
                {
                    var tempMeanList = new List<double> { statistic.Mean };
                    var meanSta = tempMeanList.toStatistic(double.Parse(meanRule.Standard),
                        double.Parse(meanRule.Upper), double.Parse(meanRule.Lower));
                    var meanDeduction = (meanSta.HighCnt + meanSta.LowCnt) * indicatorValue.MeanDeduction;
                    if (meanDeduction > 0)
                    {
                        phyRetDes.Add($"{rule.Name}平均值");
                        if (meanDeduction > indicatorValue.MeanPoints && indicatorValue.MeanPoints > 0)
                            phyRetDeduction += indicatorValue.MeanPoints;
                        else
                            phyRetDeduction += meanDeduction;
                    }
                }

                #endregion

                #region 计算 cpk 值扣分

                //if (cpkRule != null)
                //{
                //    var tempCpkList = new List<double> { statistic.Cpk };
                //    var cpkSta = tempCpkList.toStatistic(double.Parse(cpkRule.Standard),
                //        double.Parse(cpkRule.Upper), double.Parse(cpkRule.Lower));
                //    var cpkDeduction = (cpkSta.HighCnt + cpkSta.LowCnt) * indicatorValue.CvDeduction;
                //    if (cpkDeduction > 0)
                //    {
                //        phyRetDes.Add($"{rule.Name}CPK值");
                //    }
                //    if (cpkDeduction > indicatorValue.CvPoints && indicatorValue.CvPoints > 0)
                //    {
                //        phyRetDeduction += indicatorValue.CvPoints;
                //    }
                //    else
                //    {
                //        phyRetDeduction += cpkDeduction;
                //    }
                //}

                #endregion

                #region 计算 cv 值扣分

                if (cvRule != null)
                {
                    var tempCvList = new List<double> { statistic.Cv };
                    var cvSta = tempCvList.toStatistic(double.Parse(cvRule.Standard),
                        double.Parse(cvRule.Upper), double.Parse(cvRule.Lower));
                    var cvDeduction = (cvSta.HighCnt + cvSta.LowCnt) * indicatorValue.CvDeduction;
                    if (cvDeduction > 0)
                    {
                        phyRetDes.Add($"{rule.Name}CV值");
                        if (cvDeduction > indicatorValue.CvPoints && indicatorValue.CvPoints > 0)
                            phyRetDeduction += indicatorValue.CvPoints;
                        else
                            phyRetDeduction += cvDeduction;
                    }
                }

                #endregion

                if (deduction > indicatorValue.Points && indicatorValue.Points > 0)
                    phyRetDeduction += indicatorValue.Points;
                else
                    phyRetDeduction += deduction;

                if (indicatorValue.UnQualifiedOperator == UnQualifiedOperator.MoreThan)
                {
                    if (cnt > indicatorValue.UnQualifiedCount)
                    {
                        phyRetCount++;
                        phyRetDes.Add(rule.Name);
                    }
                }
                else if (indicatorValue.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual)
                {
                    if (cnt >= indicatorValue.UnQualifiedCount)
                    {
                        phyRetCount++;
                        phyRetDes.Add(rule.Name);
                    }
                }
            }

        report.PhyRetDeduction = phyRetDeduction;
        report.PhyRetDes = string.Join(",", phyRetDes);
        if (phyRetCount > 0)
        {
            report.PhyRet = ReportRet.不合格;
            report.FinalRet = ReportRet.不合格;
            report.Remark = "复检合格";
        }
        else
        {
            report.PhyRet = ReportRet.合格;
            report.FinalRet = ReportRet.合格;
            report.Remark = "";
        }

        if (isEdit)
            _prRepo.Update(report);
        else
            _prRepo.Add(report);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;
        var oldAppearance = _praRepo.All().Where(c => c.ReportId == report.Id).ToList();
        _praRepo.DeleteRange(oldAppearance);
        foreach (var appearance in appearances) appearance.ReportId = report.Id;

        _praRepo.AddRange(appearances);
        result = _uow.Save() >= 0;
        return result;
    }

    public ReportStatisticDto GetStatisticInfo(int id, out string failReason)
    {
        var report = _prRepo.Get(id);
        var mdService = _services.GetRequiredService<IMetricalDataService>();
        var groupStatistic = mdService.GetStatistic(report.GroupId, out var fail);
        var group = _gRepo.All().Include(c => c.Specification).Include(c => c.Turn).Include(c => c.MachineModel)
            .FirstOrDefault(c => c.Id == report.GroupId);
        var appearances = _praRepo.All().Include(c => c.Indicator).Where(c => c.ReportId == id).Select(c => new
        {
            c.Indicator.Name,
            c.SubScore
        }).ToList();
        var phyRetDes = !string.IsNullOrEmpty(report.PhyRetDes)
            ? report.PhyRetDes.Trim().Split(",").ToList()
            : null;
        var waterInfos = _wrRepo.All().Where(c => c.GroupId == report.GroupId).Select(c => new WaterRecordTableDto
        {
            Id = c.Id,
            TestDateTime = c.DataTestTime.ToString("yyyy-MM-dd HH:mm:ss"),
            Before = c.Before.ToString(),
            After = c.After.ToString(),
            Water = c.Water.ToString("F2")
        }).ToList();
        var dto = new ReportStatisticDto
        {
            SpecificationName = group.Specification.Name,
            TurnName = group.Turn.Name,
            MachineModelName = group.MachineModel.Name,
            StatisticColumns = groupStatistic.StatisticColumns,
            StatisticData = groupStatistic.DataInfo,
            OriginColumns = groupStatistic.OriginColumns,
            OriginData = groupStatistic.OriginDataInfo,
            WaterInfos = waterInfos,
            Appearances = appearances.Count > 0 ? appearances.Select(c => c.Name).ToList() : null,
            Points = appearances.Sum(c => c.SubScore) + report.PhyRetDeduction,
            PhyRetDes = phyRetDes,
            FinalRet = report.FinalRet.ToString()
        };

        failReason = fail;

        return dto;
    }

    public IEnumerable<ReportTableDto> GetTable(QueryInfoDto dto, out int total)
    {
        var data = _prRepo.All().AsNoTracking();
        var roleId = _accessor.HttpContext.getUserRoleId();
        var canSeeOtherData = _accessor.HttpContext.getCanSeeOtherData();
        if (roleId != _settings.AdminTypeId)
            if (!canSeeOtherData)
            {
                var userId = _accessor.HttpContext.getUserId();
                data = data.Where(c => c.Group.UserId == userId || c.Group.UserId == 0);
            }

        if (!string.IsNullOrEmpty(dto.BeginDate) && !string.IsNullOrEmpty(dto.EndDate))
        {
            var begin = Convert.ToDateTime(dto.BeginDate);
            var end = Convert.ToDateTime(dto.EndDate);
            data = data.Where(c => c.Group.BeginTime.Date >= begin &&
                                   c.Group.EndTime.Date <= end);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = Convert.ToInt32(dto.SpecificationId);
            data = data.Where(c =>
                c.Group.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var typeId = Convert.ToInt32(dto.SpecificationTypeId);
            data = data.Where(c => c.Group.Specification.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = Convert.ToInt32(dto.TurnId);
            data = data.Where(c => c.Group.TurnId == turnId);
        }

        if (!string.IsNullOrEmpty(dto.MachineModelId))
        {
            var machineModelId = Convert.ToInt32(dto.MachineModelId);
            data = data.Where(c =>
                c.Group.MachineModelId == machineModelId);
        }

        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c =>
                c.Group.Specification.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.FinalRet == (ReportRet)state);
        }

        total = data.Count();

        var list = data.OrderByDescending(c => c.Group.BeginTime).Include(c => c.Group)
            .ThenInclude(c => c.Specification)
            .Include(c => c.Group).ThenInclude(c => c.Turn)
            .Include(c => c.Group).ThenInclude(c => c.MachineModel)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new ReportTableDto
                {
                    Id = c.Id,
                    BeginTime =
                        c.Group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProductDate = c.Group.ProductionTime == null
                        ? ""
                        : Convert.ToDateTime(c.Group
                            .ProductionTime).ToString("yyyy-MM-dd"),
                    SpecificationName = c.Group.Specification.Name,
                    TurnName = c.Group.Turn.Name,
                    MachineModelName = c.Group.MachineModel.Name,
                    FinalRet = Convert.ToInt32(c.FinalRet),
                    Remark = string.IsNullOrEmpty(c.Remark) ? c.FinalRet == ReportRet.合格 ? "" : "复检合格" : c.Remark,
                    PhyScore = 100 - c.PhyRetDeduction,
                    UserName = c.Group.UserName
                }).ToList();

        return list;
    }

    public MemoryStream Download(List<int> ids)
    {
        var list = getDownloadInfo(ids, out var meaColumns, out var appColumns, out var turnName, out var typeName,
            out var date);

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("测试");
        ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        ws.Cells.Style.WrapText = true;
        ws.Cells[1, 1].Value = $"{typeName}成品检验表";
        ws.Row(1).Height = 30;
        ws.Row(1).Style.Font.Bold = true;
        ws.Column(1).Width = 25;
        ws.Column(2).Width = 35;
        ws.Cells[2, 2].Value = "班次: " + turnName;
        ws.Cells[2, 5].Value = date.ToString("yyyy年MM月dd日");
        ws.Cells[2, 5, 2, 8].Merge = true;
        ws.Cells[2, 10].Value = "编号: Q/FC - JL · ZL51 - 2019";
        ws.Cells[3, 1].Value = "机号";
        ws.Cells[3, 1, 4, 1].Merge = true;
        ws.Cells[3, 1, 4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[3, 2].Value = "品牌规格";
        ws.Cells[3, 2, 4, 2].Merge = true;
        ws.Cells[3, 2, 4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[3, 3].Value = "时间";
        ws.Cells[3, 3, 4, 3].Merge = true;
        ws.Cells[3, 3, 4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        var headerCol = 3;
        var currentCol = headerCol;
        foreach (var meaCol in meaColumns)
        {
            ws.Cells[3, currentCol].Value = meaCol;
            ws.Cells[3, currentCol, 4, currentCol].Merge = true;
            ws.Cells[3, currentCol, 4, currentCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            currentCol++;
        }

        headerCol = currentCol;
        var appCount = appColumns.Count;
        ws.Cells[3, headerCol].Value = "外观";
        ws.Cells[3, headerCol, 3, headerCol + appCount - 1].Merge = true;
        ws.Cells[3, headerCol, 3, headerCol + appCount - 1].Style.Border
            .BorderAround(ExcelBorderStyle.Thin, Color.Black);
        currentCol = headerCol++;
        foreach (var appCol in appColumns)
        {
            ws.Cells[4, currentCol].Value = appCol;
            ws.Cells[4, currentCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            currentCol++;
        }

        headerCol = currentCol;
        ws.Cells[3, headerCol].Value = "判定";
        ws.Cells[3, headerCol, 4, headerCol].Merge = true;
        ws.Cells[3, headerCol, 4, headerCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        headerCol++;
        ws.Cells[3, headerCol].Value = "综合得分";
        ws.Cells[3, headerCol, 4, headerCol].Merge = true;
        ws.Cells[3, headerCol, 4, headerCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        var col = 1;
        var row = 5;
        var lastCol = 1;
        var endCol = col + 2 + meaColumns.Count + appColumns.Count + 2;
        foreach (var item in list)
        {
            var dataCount = item.MeaInfos.Count();
            ws.Cells[row, col++].Value = item.MachineModelName;
            ws.Cells[row, col - 1, row + dataCount - 1, col - 1].Merge = true;
            ws.Cells[row, col - 1, row + dataCount - 1, col - 1].Style.Border.BorderAround
                (ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, col++].Value = item.SpecificationName;
            ws.Cells[row, col - 1, row + dataCount - 1, col - 1].Merge = true;
            ws.Cells[row, col - 1, row + dataCount - 1, col - 1].Style.Border.BorderAround
                (ExcelBorderStyle.Thin, Color.Black);
            var currentDataRow = row;
            var meaCount = appColumns.Count + meaColumns.Count + 2;
            var needMerge = true;
            var needMergeCol = "";
            var dataIndex = 1;
            var beginDataRow = row;
            foreach (var info in item.MeaInfos)
            {
                var currentDataCol = col;
                foreach (var meaCol in meaColumns)
                {
                    var value = "";
                    var merge = false;
                    if (info.Any(c => c.Key == meaCol))
                    {
                        if (needMerge)
                        {
                            var hasValueCount = item.MeaInfos.Count(c =>
                                c.Keys.Contains(meaCol) && c[meaCol].ToString() != "");
                            if (hasValueCount == 1) merge = true;
                        }

                        value = info[meaCol].ToString();
                    }

                    ws.Cells[currentDataRow, currentDataCol].Style.Border.BorderAround(ExcelBorderStyle.Thin,
                        Color.Black);
                    ws.Cells[currentDataRow, currentDataCol++].Value = value;
                    if (dataIndex == dataCount)
                    {
                        if (merge)
                        {
                            ws.Cells[beginDataRow, currentDataCol, beginDataRow + dataCount - 1, currentDataCol]
                                .Merge = true;
                            ws.Cells[beginDataRow, currentDataCol, beginDataRow + dataCount - 1, currentDataCol]
                                .Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            merge = false;
                        }
                    }
                    else
                    {
                        ws.Cells[currentDataRow, currentDataCol].Style.Border.BorderAround(ExcelBorderStyle.Thin,
                            Color.Black);
                    }

                    needMerge = false;
                }

                foreach (var appCol in appColumns)
                {
                    var value = "";
                    if (info.Any(c => c.Key == appCol)) value = info[appCol].ToString();

                    ws.Cells[currentDataRow, currentDataCol++].Value = value;
                    if (dataIndex == dataCount)
                    {
                        ws.Cells[beginDataRow, currentDataCol - 1, beginDataRow + dataCount - 1, currentDataCol - 1]
                            .Merge = true;
                        ws.Cells[beginDataRow, currentDataCol - 1, beginDataRow + dataCount - 1, currentDataCol - 1]
                            .Style.Border.BorderAround(ExcelBorderStyle.Thin,
                                Color.Black);
                    }
                }

                var total = info["判定"];
                ws.Cells[currentDataRow, currentDataCol++].Value = total == null ? "" : total.ToString();
                if (dataIndex == dataCount)
                {
                    ws.Cells[beginDataRow, currentDataCol, beginDataRow + dataCount - 1, currentDataCol]
                        .Merge = true;
                    ws.Cells[beginDataRow, currentDataCol, beginDataRow + dataCount - 1, currentDataCol]
                        .Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                }
                else
                {
                    ws.Cells[currentDataRow, currentDataCol].Style.Border.BorderAround(ExcelBorderStyle.Thin,
                        Color.Black);
                }

                var result = info["综合得分"];
                ws.Cells[currentDataRow, currentDataCol++].Value = result == null ? "" : result.ToString();
                if (dataIndex == dataCount)
                {
                    ws.Cells[beginDataRow, currentDataCol - 2, beginDataRow + dataCount - 1, currentDataCol - 2]
                        .Merge = true;
                    ws.Cells[beginDataRow, currentDataCol - 2, beginDataRow + dataCount - 1, currentDataCol - 2]
                        .Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                }
                else
                {
                    ws.Cells[currentDataRow, currentDataCol - 2].Style.Border.BorderAround(ExcelBorderStyle.Thin,
                        Color.Black);
                }

                currentDataRow++;
                dataIndex++;
            }

            row += dataCount;
            col = 1;
        }

        ws.Cells[row, 12].Value = "检验员:";
        ws.Cells[row, 12, row, endCol - 1].Merge = true;

        ws.Cells[1, 1, 1, endCol - 1].Merge = true;
        ws.Cells[2, 10, 2, endCol - 1].Merge = true;
        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }

    private IEnumerable<ReportDownloadDto> getDownloadInfo(List<int> reportIds, out List<string> meaColumns,
        out List<string> appColumns, out string turnName, out
            string
            typeName, out DateTime date)
    {
        // 物测指标表头
        meaColumns = new List<string> { "时间" };
        // 外观指标表头
        appColumns = new List<string>();
        turnName = string.Empty;
        typeName = string.Empty;
        date = DateTime.Now;
        var reports = _prRepo.All().AsNoTracking().Include(c => c.Group).ThenInclude(c => c.Turn)
            .Where(c => reportIds.Contains(c.Id));
        var first = reports.FirstOrDefault();
        if (first == null) return null;

        var specification = _spRepo.All().Include(c => c.SpecificationType)
            .FirstOrDefault(c => c.Id == first.Group.SpecificationId);
        typeName = specification.SpecificationType.Name;
        turnName = first.Group.Turn.Name;
        date = first.Group.BeginTime;

        var reportList = reports.ToList();
        var list = new List<ReportDownloadDto>();
        // 获取牌号下的测量指标
        var singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules).ToList();
        // 提取其中的外观指标
        var appIndicators = singleRules.Where(c => c.Standard == "0").ToList();
        foreach (var appIndicator in appIndicators)
        {
            var appColumn = "";
            // 由于牌号规则中有些指标没有保存指标名称, 所以要先判断是否有指标名称
            if (appIndicator.Name == "")
            {
                var indicator = _iRepo.Get(appIndicator.Id);
                appColumn = indicator.Name;
            }
            else
            {
                appColumn = appIndicator.Name;
            }

            appColumns.Add(appColumn);
        }

        var groupIds = reportList.Select(c => c.GroupId).ToList();
        var groupList = _gRepo.All().Include(c => c.Specification).Include(c => c.MachineModel)
            .Where(c => groupIds.Contains(c.Id)).ToList();
        var initColumns = false;
        foreach (var g in groupList)
        {
            var dataList = _dRepo.All().Where(c => c.GroupId == g.Id).ToList();
            // 排除没有测量数据的组
            if (dataList.Count == 0) continue;

            var report = reports.FirstOrDefault(c => c.GroupId == g.Id);
            var appSubScore = _praRepo.All().Where(c => c.ReportId == report.Id).Sum(c => c.SubScore);

            var dto = new ReportDownloadDto
            {
                SpecificationName = g.Specification.Name,
                MachineModelName = g.MachineModel.Name
            };
            var meaInfos = new List<Dictionary<string, object>>();
            var totalList = new List<int>();
            foreach (var data in dataList)
            {
                var meaInfo = new Dictionary<string, object>();
                // 获取测量具体数据
                var obj = (JObject)JsonConvert.DeserializeObject(data.Data);
                // 获取测量时间
                var testTime = Convert.ToDateTime(obj["testTime"].ToString());
                meaInfo.Add("时间", testTime.ToString("HH:mm"));
                // 遍历牌号规则中的指标分离测量数据中的指标和值
                foreach (var rule in singleRules)
                {
                    var indicatorName = rule.Name;
                    if (string.IsNullOrEmpty(indicatorName)) indicatorName = _iRepo.Get(rule.Id).Name;

                    var value = obj[$"{rule.Id}"];
                    // 外观指标
                    if (value != null)
                    {
                        meaInfo.Add(indicatorName, value.ToString());
                        if (!initColumns)
                            if (rule.Standard != "0")
                                meaColumns.Add(indicatorName);
                    }
                }

                initColumns = true;
                totalList.Add(data.Total);
                meaInfo.Add("判定", report.FinalRet.ToString());
                meaInfo.Add("综合得分", 100 - report.PhyRetDeduction - appSubScore);

                meaInfos.Add(meaInfo);
            }

            dto.MeaInfos = meaInfos;
            dto.MeanTotal = totalList.Average();
            list.Add(dto);
        }

        return list;
    }
}