using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.WrapQualityInspectionReport;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using NotImplementedException = System.NotImplementedException;

namespace Infrastructure.Services.Reports.Impl;

[AutoInject(typeof(IWrapQualityInspectionReportService), InjectType.Scope)]
public class WrapQualityInspectionReportService: IWrapQualityInspectionReportService
{
    private readonly IRepository<WrapQualityInspectionReport> _wiRepo;
    private readonly IRepository<WrapQualityInspectionReportDefect> _widRepo;
    private readonly IUnitOfWork _uow;

    public WrapQualityInspectionReportService(IRepository<WrapQualityInspectionReport> wiRepo,
        IRepository<WrapQualityInspectionReportDefect> widRepo,
        IUnitOfWork uow)
    {
        _wiRepo = wiRepo;
        _widRepo = widRepo;
        _uow = uow;
    }
    public IEnumerable<WrapQualityInspectionReportInfoDto> GetReport(WrapQualityInspectionReportQueryInfoDto dto, out int total)
    {
        var data = _wiRepo.All().AsNoTracking();

        if (!string.IsNullOrEmpty(dto.BeginTime) && !string.IsNullOrEmpty(dto.EndTime))
        {
            var begin = Convert.ToDateTime(dto.BeginTime);
            var end = Convert.ToDateTime(dto.EndTime);
            data = data.Where(c => c.Time.Date >= begin && c.Time.Date <= end);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.TeamId))
        {
            var teamId = int.Parse(dto.TeamId);
            data = data.Where(c => c.TeamId == teamId);
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = int.Parse(dto.TurnId);
            data = data.Where(c => c.TurnId == turnId);
        }

        if (!string.IsNullOrEmpty(dto.VolumePickUpId))
        {
            var volumePickUpId = int.Parse(dto.VolumePickUpId);
            data = data.Where(c => c.VolumePickUpId == volumePickUpId);
        }

        if (!string.IsNullOrEmpty(dto.PackagingMachineId))
        {
            var packagingMachineId = int.Parse(dto.PackagingMachineId);
            data = data.Where(c => c.PackagingMachineId == packagingMachineId);
        }

        if (!string.IsNullOrEmpty(dto.Result))
        {
            var result = int.Parse(dto.Result);
            data = data.Where(c => c.Result == (QualityResult)result);
        }

        total = data.Count();

        var list = data.OrderByDescending(c => c.Time).Skip(dto.Skip())
            .Take(dto.PageSize).Select(c => new WrapQualityInspectionReportInfoDto()
            {
                Id = c.Id, Time = c.Time.ToString("yyyy-MM-dd"), SpecificationId = c.SpecificationId,
                SpecificationName = c.Specification.Name, TeamId = c.TeamId, TeamName = c.Team.Name,
                TurnId = c.TurnId, TurnName = c.Turn.Name, VolumePickUpId = c.VolumePickUpId,
                VolumePickUpName = c.VolumePickUp.Name, PackagingMachineId = c.PackagingMachineId,
                PackagingMachineName = c.PackagingMachine.Name, Count = c.InspectionCount, Result = (int)c.Result,
                OrderNo = c.OrderNo, Inspector = c.Inspector, VolumePickUpOperator = c.VolumePickUpOperator,
                PackagingMachineOperator = c.PackagingMachineOperator
            }).ToList();

        return list;
    }

    public bool EditReport(WrapQualityInspectionReportInfoDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var report = new WrapQualityInspectionReport();
        if (modify)
        {
            report = _wiRepo.Get(dto.Id);
        }

        report.Time = Convert.ToDateTime(dto.Time);
        report.SpecificationId = dto.SpecificationId;
        report.TeamId = dto.TeamId;
        report.TurnId = dto.TurnId;
        report.VolumePickUpId = dto.VolumePickUpId;
        report.PackagingMachineId = dto.PackagingMachineId;
        report.InspectionCount = dto.Count;
        report.OrderNo = dto.OrderNo;
        report.Inspector = dto.Inspector;
        report.VolumePickUpOperator = dto.VolumePickUpOperator;
        report.PackagingMachineOperator = dto.PackagingMachineOperator;
        report.Result = QualityResult.Quality;
        
        if (modify)
            _wiRepo.Update(report);
        else
            _wiRepo.Add(report);

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;
        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";
        return result;
    }

    public bool RemoveReport(List<int> ids, out string message)
    {
        var reports = _wiRepo.All().Where(c => ids.Contains(c.Id)).ToList();
        _wiRepo.DeleteRange(reports);
        var result = _uow.Save() >= ids.Count;
        message = result ? "删除成功" : "删除失败";
        return result;
    }
}