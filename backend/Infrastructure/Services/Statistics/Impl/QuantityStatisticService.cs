using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Dtos.Specification;
using Core.Dtos.QuantityStatistic;
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

public class QuantityStatisticService : IQuantityStatisticService
{
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;

    public QuantityStatisticService(IRepository<Group> gRepo,
        IRepository<Data> dRepo,
        IRepository<Core.Entities.Specification> spRepo,
        IOptionsSnapshot<Settings> settings)
    {
        _gRepo = gRepo;
        _dRepo = dRepo;
        _spRepo = spRepo;
        _settings = settings.Value;
    }


    public IEnumerable<QuantityStatisticInfoDto> GetInfo(QuantityStatisticQueryInfoDto dto)
    {
        var data = getInfo(dto);
        return data;
    }

    public MemoryStream Download(QuantityStatisticQueryInfoDto dto)
    {
        var data = getInfo(dto);
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("定量统计分析");
        ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        ws.Row(1).Height = 25;
        ws.Column(2).Width = 40;
        ws.Column(11).Width = 25;
        ws.Cells[1, 1].Value = "定量统计分析";
        ws.Cells[1, 1, 1, 12].Merge = true;
        ws.Cells[1, 1, 1, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 1].Value = "序号";
        ws.Cells[2, 2].Value = "牌号名称";
        ws.Cells[2, 3].Value = "均值偏差";
        ws.Cells[2, 4].Value = "平均值";
        ws.Cells[2, 5].Value = "最大值";
        ws.Cells[2, 6].Value = "最小值";
        ws.Cells[2, 7].Value = "SD";
        ws.Cells[2, 8].Value = "Cpk";
        ws.Cells[2, 9].Value = "Cv";
        ws.Cells[2, 10].Value = "Offset";
        ws.Cells[2, 11].Value = "合格数";
        ws.Cells[2, 12].Value = "合格率";
        ws.Cells[2, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[2, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

        var row = 3;
        var index = 1;
        foreach (var item in data)
        {
            ws.Cells[row, 1].Value = index;
            ws.Cells[row, 2].Value = item.SpecificationName;
            ws.Cells[row, 3].Value = item.MeanOffset;
            ws.Cells[row, 4].Value = item.Mean;
            ws.Cells[row, 5].Value = item.Max;
            ws.Cells[row, 6].Value = item.Min;
            ws.Cells[row, 7].Value = item.Sd;
            ws.Cells[row, 8].Value = item.Cpk;
            ws.Cells[row, 9].Value = item.Cv;
            ws.Cells[row, 10].Value = item.Offs;
            ws.Cells[row, 11].Value = item.Quality;
            ws.Cells[row, 12].Value = item.Rate;
            ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            row++;
            index++;
        }

        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }

    private IEnumerable<QuantityStatisticInfoDto> getInfo(QuantityStatisticQueryInfoDto dto)
    {
        var temp = _gRepo.All().Where(c => c.MeasureTypeId == _settings.ProductionTypeId).AsNoTracking();
        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            temp = temp.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var typeId = int.Parse(dto.SpecificationTypeId);
            temp = temp.Where(c => c.Specification.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.BeginDate) && !string.IsNullOrEmpty(dto.EndDate))
        {
            var begin = Convert.ToDateTime(dto.BeginDate);
            var end = Convert.ToDateTime(dto.EndDate);
            temp = temp.Where(c => c.BeginTime.Date >= begin && c.EndTime.Date <= end);
        }

        var tempList = temp.Include(c => c.Specification).ToList();
        var specificationGroups = tempList.GroupBy(c => c.SpecificationId).ToList();
        var list = new List<QuantityStatisticInfoDto>();
        var specifications = _spRepo.All().Select(c => new { c.Id, c.Name, c.SingleRules }).ToList();
        foreach (var groups in specificationGroups)
        {
            var tempQuantityList = new List<double>();
            var groupIds = groups.Select(c => c.Id).ToList();
            var tempData = _dRepo.All().Where(c => groupIds.Contains(c.GroupId)).ToList();
            var specification = specifications.FirstOrDefault(c => c.Id == groups.Key);
            if (specification == null)
                continue;
            foreach (var g in groups)
            {
                var tempDataList = tempData.Where(c => c.GroupId == g.Id).Select(c => c.Data).ToList();
                foreach (var data in tempDataList)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(data);
                    double quantity = 0;
                    if (obj[_settings.QuantityIndicatorId.ToString()] != null &&
                        !string.IsNullOrEmpty(obj[_settings.QuantityIndicatorId.ToString()].ToString()))
                        quantity = double.Parse(obj[_settings.QuantityIndicatorId.ToString()].ToString());

                    if (quantity > 0) tempQuantityList.Add(quantity);
                }
            }

            if (tempQuantityList.Count == 0)
                continue;
            var rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules).ToList();
            var quantityRule = rules.FirstOrDefault(c => c.Id == _settings.QuantityIndicatorId);
            if (quantityRule == null)
                continue;
            var statistic = tempQuantityList.toStatistic(double.Parse(quantityRule.Standard),
                double.Parse(quantityRule.Upper), double.Parse(quantityRule.Lower), equal: quantityRule.Equal);
            var item = new QuantityStatisticInfoDto
            {
                SpecificationName = specification.Name,
                Min = statistic.Min.ToString("F2"),
                Max = statistic.Max.ToString("F2"),
                Mean = statistic.Mean.ToString("F2"),
                MeanOffset = statistic.MeanOffset.ToString("F4"),
                Sd = statistic.Sd.ToString("F2"),
                Cpk = statistic.Cpk.ToString("F2"),
                Cv = statistic.Cv.ToString("F2"),
                Offs = statistic.Offset.ToString("F2"),
                Quality = statistic.Quality,
                Rate = statistic.QualityRate
            };
            list.Add(item);
        }

        return list;
    }
}