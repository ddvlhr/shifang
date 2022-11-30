using System;
using System.IO;
using System.Linq;
using Core.Dtos.ManualInspectionReport;
using Core.Dtos.ManualTestStatistic;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using NotImplementedException = System.NotImplementedException;

namespace Infrastructure.Services.Statistics.Impl;

[AutoInject(typeof(IManualTestStatisticService), InjectType.Scope)]
public class ManualTestStatisticService: IManualTestStatisticService
{
    private readonly IRepository<ManualInspectionReport> _manualInspectionReportRepo;

    public ManualTestStatisticService(
        IRepository<ManualInspectionReport> manualInspectionReportRepo)
    {
        _manualInspectionReportRepo = manualInspectionReportRepo;
    }
    public ManualTestStatisticInfoDto Search(ManualInspectionReportQueryInfoDto dto)
    {
        return getStatistic(dto);
    }

    public MemoryStream Download(ManualInspectionReportQueryInfoDto dto)
    {
        throw new NotImplementedException();
    }

    private ManualTestStatisticInfoDto getStatistic(ManualInspectionReportQueryInfoDto dto)
    {
        var data = _manualInspectionReportRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End))
        {
            var begin = Convert.ToDateTime(dto.Begin);
            var end = Convert.ToDateTime(dto.End);
            data = data.Where(c => c.Time.Date >= begin && c.Time.Date <= end);
        }

        var list = data.ToList();
        var result = list.GroupBy(c => c.Result).Select(c => new ManualTestStatisticInfoDto.TableItem()
        {
            Result = c.Key.toDescription(), Count = c.Count()
        }).ToList();

        var info = new ManualTestStatisticInfoDto();
        info.TableInfo = result;

        return info;
    }
}