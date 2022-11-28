using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Dtos.Specification;
using Core.Dtos.Statistics;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Services.Statistics.Impl;

public class FilterMeasureQualityService : IFilterMeasureQualityService
{
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;

    public FilterMeasureQualityService(IRepository<Group> gRepo,
        IRepository<Data> dRepo,
        IOptionsSnapshot<Settings> settings,
        IRepository<Core.Entities.Specification> spRepo)
    {
        _gRepo = gRepo;
        _dRepo = dRepo;
        _spRepo = spRepo;
        _settings = settings.Value;
    }

    public IEnumerable<FilterMeasureQualityInfoDto> Search(StatisticQueryInfoDto dto)
    {
        var data = getDataInfo(dto);
        return data;
    }

    public MemoryStream Download(StatisticQueryInfoDto dto)
    {
        var data = getDataInfo(dto);
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("滤棒检测合格率");
        ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        var headers = new List<string>
        {
            "牌号", "重量单支不合格数", "重量单支合格率", "圆周单支不合格数", "圆周单支合格率",
            "圆度单支不合格数", "圆度单支合格率", "长度单支不合格数", "长度单支合格率",
            "压降单支不合格数", "压降单支合格率", "硬度单支不合格数", "硬度单支合格率"
        };

        var headerCol = 1;
        ws.Row(1).Height = 25;
        foreach (var header in headers)
        {
            ws.Cells[1, headerCol].Value = header;
            ws.Cells[1, headerCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            if (headerCol > 1)
                ws.Column(headerCol).Width = 25;
            else
                ws.Column(headerCol).Width = 30;

            headerCol++;
        }

        var row = 2;
        var col = 1;
        foreach (var item in data)
        {
            ws.Row(row).Height = 20;
            ws.Cells[row, 1].Value = item.SpecificationName;
            ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 2].Value = item.WeightQuality;
            ws.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 3].Value = item.WeightQualityRate;
            ws.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 4].Value = item.CircleQuality;
            ws.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 5].Value = item.CircleQualityRate;
            ws.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 6].Value = item.OvalQuality;
            ws.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 7].Value = item.OvalQualityRate;
            ws.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 8].Value = item.LengthQuality;
            ws.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 9].Value = item.LengthQualityRate;
            ws.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 10].Value = item.ResistanceQuality;
            ws.Cells[row, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 11].Value = item.ResistanceQualityRate;
            ws.Cells[row, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 12].Value = item.HardnessQuality;
            ws.Cells[row, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 13].Value = item.HardnessQualityRate;
            ws.Cells[row, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            row++;
        }

        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }

    private IEnumerable<FilterMeasureQualityInfoDto> getDataInfo(StatisticQueryInfoDto dto)
    {
        var result = new List<FilterMeasureQualityInfoDto>();
        var data = _gRepo.All().AsNoTracking()
            .Where(c => c.Specification.SpecificationTypeId == _settings.FilterTypeId);
        data = data.Where(c => c.MeasureTypeId == _settings.ProductionTypeId);
        var specifications = _spRepo.All().AsNoTracking().Select(c => new { c.Id, c.SingleRules, c.MeanRules });
        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.BeginDate) && !string.IsNullOrEmpty(dto.EndDate))
        {
            var begin = Convert.ToDateTime(dto.BeginDate);
            var end = Convert.ToDateTime(dto.EndDate);
            data = data.Where(c => c.BeginTime.Date >= begin &&
                                   c.BeginTime.Date <= end);
        }

        if (!string.IsNullOrEmpty(dto.ExcludeDate))
        {
            var excludeDates = new List<DateTime>();
            foreach (var date in dto.ExcludeDate.Split(",", StringSplitOptions.RemoveEmptyEntries))
                excludeDates.Add(Convert.ToDateTime(date).Date);
            data = data.Where(c => !excludeDates.Contains(c.BeginTime.Date));
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = int.Parse(dto.TurnId);
            data = data.Where(c => c.TurnId == turnId);
        }

        // if (!string.IsNullOrEmpty(dto.MachineId))
        // {
        //     var machineIds = new List<int>();
        //     foreach (var s in dto.MachineId.Split(",", StringSplitOptions.RemoveEmptyEntries))
        //         machineIds.Add(int.Parse(s));
        //     data = data.Where(c => machineIds.Contains(c.MachineModelId));
        // }

        var specificationGroups = data.Include(c => c.Specification).ToList().GroupBy(c => c.SpecificationId).ToList();
        foreach (var specificationGroup in specificationGroups)
        {
            var first = specificationGroup.FirstOrDefault();
            if (first == null) continue;

            var item = new FilterMeasureQualityInfoDto
            {
                SpecificationName = first.Specification.Name
            };
            var specification = specifications.FirstOrDefault(c => c.Id == specificationGroup.Key);
            var singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
            var meanRules = JsonConvert.DeserializeObject<List<Rule>>(specification.MeanRules);
            var groupIds = specificationGroup.Select(c => c.Id).ToList();
            var tempDataList = _dRepo.All().Where(c => groupIds.Contains(c.GroupId)).ToList();
            var tempData = getFilterMeasureDataList(tempDataList);
            if (tempData.WeightList.Count > 0)
            {
                var total = tempData.WeightList.Count;
                var singleWeightRule = singleRules.FirstOrDefault(c => c.Id == _settings.Weight);
                var sta = tempData.WeightList.toStatistic(double.Parse(singleWeightRule.Standard),
                    double.Parse(singleWeightRule.Upper),
                    double.Parse(singleWeightRule.Lower));
                var un = sta.HighCnt + sta.LowCnt;
                item.WeightQuality = $"{un} / {total}";
                item.WeightQualityRate = ((total - un) / Convert.ToDouble(total) * 100)
                    .ToString("F2") + " %";
            }

            if (tempData.CircleList.Count > 0)
            {
                var total = tempData.CircleList.Count;
                var singleCircleRule = singleRules.FirstOrDefault(c => c.Id == _settings.Circle);
                var sta = tempData.CircleList.toStatistic(double.Parse(singleCircleRule.Standard),
                    double.Parse(singleCircleRule.Upper),
                    double.Parse(singleCircleRule.Lower));
                var un = sta.HighCnt + sta.LowCnt;
                item.CircleQuality = $"{un} / {total}";
                item.CircleQualityRate = ((total - un) / Convert.ToDouble(total) * 100)
                    .ToString("F2") + " %";
            }

            if (tempData.OvalList.Count > 0)
            {
                var total = tempData.OvalList.Count;
                var singleOvalRule = singleRules.FirstOrDefault(c => c.Id == _settings.Oval);
                var sta = tempData.OvalList.toStatistic(double.Parse(singleOvalRule.Standard),
                    double.Parse(singleOvalRule.Upper),
                    double.Parse(singleOvalRule.Lower));
                var un = sta.HighCnt + sta.LowCnt;
                item.OvalQuality = $"{un} / {total}";
                item.OvalQualityRate = ((total - un) / Convert.ToDouble(total) * 100)
                    .ToString("F2") + " %";
            }

            if (tempData.LengthList.Count > 0)
            {
                var total = tempData.LengthList.Count;
                var singleLengthRule = singleRules.FirstOrDefault(c => c.Id == _settings.Length);
                var sta = tempData.LengthList.toStatistic(double.Parse(singleLengthRule.Standard),
                    double.Parse(singleLengthRule.Upper),
                    double.Parse(singleLengthRule.Lower));
                var un = sta.HighCnt + sta.LowCnt;
                item.LengthQuality = $"{un} / {total}";
                item.LengthQualityRate = ((total - un) / Convert.ToDouble(total) * 100)
                    .ToString("F2") + " %";
            }

            if (tempData.ResistanceList.Count > 0)
            {
                var total = tempData.ResistanceList.Count;
                var singleResistanceRule = singleRules.FirstOrDefault(c => c.Id == _settings.Resistance);
                var sta = tempData.ResistanceList.toStatistic(double.Parse(singleResistanceRule.Standard),
                    double.Parse(singleResistanceRule.Upper),
                    double.Parse(singleResistanceRule.Lower));
                var un = sta.HighCnt + sta.LowCnt;
                item.ResistanceQuality = $"{un} / {total}";
                item.ResistanceQualityRate = ((total - un) / Convert.ToDouble(total) * 100)
                    .ToString("F2") + " %";
            }

            if (tempData.HardnessList.Count > 0)
            {
                var total = tempData.HardnessList.Count;
                var singleHardnessRule = singleRules.FirstOrDefault(c => c.Id == _settings.Hardness);
                var sta = tempData.HardnessList.toStatistic(double.Parse(singleHardnessRule.Standard),
                    double.Parse(singleHardnessRule.Upper),
                    double.Parse(singleHardnessRule.Lower));
                var un = sta.HighCnt + sta.LowCnt;
                item.HardnessQuality = $"{un} / {total}";
                item.HardnessQualityRate = ((total - un) / Convert.ToDouble(total) * 100)
                    .ToString("F2") + " %";
            }

            result.Add(item);
        }

        return result;
    }

    private TempData getFilterMeasureDataList(IEnumerable<Data> dataList)
    {
        var weightList = new List<double>();
        var circleList = new List<double>();
        var ovalList = new List<double>();
        var lengthList = new List<double>();
        var resistanceList = new List<double>();
        var hardnessList = new List<double>();
        foreach (var item in dataList)
        {
            var data = JsonConvert.DeserializeObject<JObject>(item.Data);
            if (data[$"{_settings.Weight}"] != null)
            {
                var value = data[$"{_settings.Weight}"].ToString();
                if (!string.IsNullOrEmpty(value)) weightList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Circle}"] != null)
            {
                var value = data[$"{_settings.Circle}"].ToString();
                if (!string.IsNullOrEmpty(value)) circleList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Oval}"] != null)
            {
                var value = data[$"{_settings.Oval}"].ToString();
                if (!string.IsNullOrEmpty(value)) ovalList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Length}"] != null)
            {
                var value = data[$"{_settings.Length}"].ToString();
                if (!string.IsNullOrEmpty(value)) lengthList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Resistance}"] != null)
            {
                var value = data[$"{_settings.Resistance}"].ToString();
                if (!string.IsNullOrEmpty(value)) resistanceList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Hardness}"] != null)
            {
                var value = data[$"{_settings.Hardness}"].ToString();
                if (!string.IsNullOrEmpty(value)) hardnessList.Add(double.Parse(value));
            }
        }

        var tempData = new TempData
        {
            WeightList = weightList,
            CircleList = circleList,
            OvalList = ovalList,
            LengthList = lengthList,
            ResistanceList = resistanceList,
            HardnessList = hardnessList
        };
        return tempData;
    }

    public class TempData
    {
        public List<double> WeightList { get; set; }
        public List<double> CircleList { get; set; }
        public List<double> OvalList { get; set; }
        public List<double> LengthList { get; set; }
        public List<double> ResistanceList { get; set; }
        public List<double> HardnessList { get; set; }
    }

    public class ReturnData
    {
        public string SpecificationName { get; set; }
        public string WeightQuality { get; set; }
        public string WeightQualityRate { get; set; }
        public string CircleQuality { get; set; }
        public string CircleQualityRate { get; set; }
        public string OvalQuality { get; set; }
        public string OvalQualityRate { get; set; }
        public string LengthQuality { get; set; }
        public string LengthQualityRate { get; set; }
        public string ResistanceQuality { get; set; }
        public string ResistanceQualityRate { get; set; }
        public string HardnessQuality { get; set; }
        public string HardnessQualityRate { get; set; }
    }
}