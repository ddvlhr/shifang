using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Dtos.AppearanceStatistic;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Services.Statistics.Impl;

public class AppearanceStatisticService : IAppearanceStatisticService
{
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly Settings _settings;

    public AppearanceStatisticService(IRepository<Group> gRepo, IRepository<Data> dRepo,
        IOptionsSnapshot<Settings> settings,
        IRepository<Core.Entities.Indicator> iRepo)
    {
        _gRepo = gRepo;
        _dRepo = dRepo;
        _iRepo = iRepo;
        _settings = settings.Value;
    }

    public IEnumerable<AppearanceStatisticInfoDto> GetInfo(QueryInfoDto dto)
    {
        var data = _dRepo.All().Where(c => c.Group.MeasureTypeId == _settings.ProductionTypeId).AsNoTracking();
        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var typeId = int.Parse(dto.SpecificationTypeId);
            data = data.Where(c => c.Group.Specification.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.BeginDate) && !string.IsNullOrEmpty(dto.EndDate))
        {
            var begin = Convert.ToDateTime(dto.BeginDate);
            var end = Convert.ToDateTime(dto.EndDate);
            data = data.Where(c => c.Group.BeginTime.Date >= begin && c.Group.EndTime.Date <= end);
        }

        var list = new List<AppearanceStatisticInfoDto>();
        var indicators = _iRepo.All().Where(c => c.IndicatorProject == IndicatorProject.Appearance).ToList();

        foreach (var item in data)
        {
            var tempList = new List<AppearanceStatisticInfoDto>();
            foreach (var indicator in indicators)
            {
                var obj = JsonConvert.DeserializeObject<JObject>(item.Data);
                if (obj[$"{indicator.Id}"] != null)
                {
                    var value = obj[$"{indicator.Id}"].ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        var tempItem = new AppearanceStatisticInfoDto
                        {
                            Name = indicator.Name,
                            Count = int.Parse(value)
                        };
                        tempList.Add(tempItem);
                    }
                }
            }

            if (tempList.Count > 0) list.AddRange(tempList);
        }

        var result = new List<AppearanceStatisticInfoDto>();
        var total = list.Sum(c => c.Count);
        var groups = list.GroupBy(c => c.Name).ToList();
        foreach (var g in groups)
        {
            var count = g.Sum(c => c.Count);
            var percent = (count / Convert.ToDouble(total) * 100).ToString("F2") + "%";
            var item = new AppearanceStatisticInfoDto
            {
                Name = g.Key,
                Percent = percent,
                Count = count
            };
            result.Add(item);
        }

        return result;
    }

    public MemoryStream DownloadData(QueryInfoDto dto)
    {
        var data = GetInfo(dto).ToList();
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("外观统计数据");
        ws.Column(1).Width = 20;
        ws.Column(2).Width = 20;
        ws.Column(3).Width = 20;
        ws.Row(1).Height = 20;
        ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        ws.Cells[1, 1].Value = "外观项目";
        ws.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 2].Value = "频次";
        ws.Cells[1, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
        ws.Cells[1, 3].Value = "占比";
        ws.Cells[1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

        var row = 2;
        foreach (var item in data)
        {
            ws.Row(row).Height = 20;
            ws.Cells[row, 1].Value = item.Name;
            ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 2].Value = item.Count;
            ws.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[row, 3].Value = item.Percent;
            ws.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            row++;
        }

        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }
}