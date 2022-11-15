using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.Report;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QueryInfoDto = Core.Dtos.MonthCraftReport.QueryInfoDto;

namespace Infrastructure.Services.Reports.Impl;

public class MonthCraftReportService : IMonthCraftReportService
{
    private readonly IRepository<MonthCraftReport> _mcrRepo;
    private readonly IUnitOfWork _uow;

    public MonthCraftReportService(IRepository<MonthCraftReport> mcrRepo, IUnitOfWork uow)
    {
        _mcrRepo = mcrRepo;
        _uow = uow;
    }

    public IEnumerable<MonthCraftReportTableDto> GetTable(QueryInfoDto dto, out int total)
    {
        var data = _mcrRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c => c.PartName.Contains(dto.Query) ||
                                   c.User.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.WorkShop))
            data = data.Where(c => c.PartName == dto.WorkShop);

        if (!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End))
        {
            var begin = Convert.ToDateTime(dto.Begin);
            var end = Convert.ToDateTime(dto.End);
            data = data.Where(c => c.Time.Date >= begin &&
                                   c.Time.Date <= end);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.Time).Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
            new MonthCraftReportTableDto
            {
                Id = c.Id, PartName = c.PartName, User = c.User, Time = c.Time.ToString("yyyy-MM-dd")
            }).ToList();

        return result;
    }

    public bool Add(MonthCraftReportEditDto dto, out string failReason)
    {
        failReason = string.Empty;

        var monthCraftReport = new MonthCraftReport
        {
            PartName = dto.PartName,
            User = dto.User,
            Time = Convert.ToDateTime(dto.Time),
            Score = dto.Score,
            Result = JsonConvert.SerializeObject(dto.Result)
        };

        _mcrRepo.Add(monthCraftReport);

        return _uow.Save() > 0;
    }

    public bool Update(MonthCraftReportEditDto dto, out string failReason)
    {
        failReason = string.Empty;

        var monthCraftReport = _mcrRepo.Get(dto.Id);
        if (monthCraftReport == null)
        {
            failReason = "没有找到对应的报表";
            return false;
        }

        monthCraftReport.PartName = dto.PartName;
        monthCraftReport.User = dto.User;
        monthCraftReport.Time = Convert.ToDateTime(dto.Time);
        monthCraftReport.Score = dto.Score;
        monthCraftReport.Result = JsonConvert.SerializeObject(dto.Result);

        _mcrRepo.Update(monthCraftReport);

        return _uow.Save() > 0;
    }

    public MonthCraftReportEditDto GetInfo(int id, out string failReason)
    {
        var dto = new MonthCraftReportEditDto();
        failReason = string.Empty;

        var report = _mcrRepo.Get(id);
        if (report == null)
        {
            failReason = "没有找到对应的报表";
            return dto;
        }

        dto.Id = report.Id;
        dto.PartName = report.PartName;
        dto.Score = report.Score;
        dto.User = report.User;
        dto.TimeStr = report.Time.ToString("yyyy年MM月dd日");
        dto.Time = report.Time.ToString("yyyy-MM-dd");
        dto.Result = JsonConvert.DeserializeObject<List<MonthCraftReportItem>>(report.Result);

        return dto;
    }

    public bool Remove(List<int> ids, out string failReason)
    {
        failReason = string.Empty;
        var data = _mcrRepo.All().Where(c => ids.Contains(c.Id)).ToList();
        _mcrRepo.DeleteRange(data);
        return _uow.Save() > 0;
    }
}