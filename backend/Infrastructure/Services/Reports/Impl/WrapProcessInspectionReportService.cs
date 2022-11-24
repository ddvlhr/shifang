using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Dtos;
using Core.Dtos.WrapProcessInspectionReport;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using NotImplementedException = System.NotImplementedException;

namespace Infrastructure.Services.Reports.Impl;

[AutoInject(typeof(IWrapProcessInspectionReportService), InjectType.Scope)]
public class WrapProcessInspectionReportService: IWrapProcessInspectionReportService
{
    private readonly IRepository<WrapProcessInspectionReport> _wpRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<WrapProcessInspectionReportDefect> _wpdRepo;
    private readonly IRepository<Core.Entities.Defect> _dRepo;

    public WrapProcessInspectionReportService(IRepository<WrapProcessInspectionReport> wpRepo,
        IUnitOfWork uow,
        IRepository<WrapProcessInspectionReportDefect> wpdRepo,
        IRepository<Core.Entities.Defect> dRepo)
    {
        _wpRepo = wpRepo;
        _uow = uow;
        _wpdRepo = wpdRepo;
        _dRepo = dRepo;
    }
    public IEnumerable<WrapProcessInspectionReportInfoDto> GetReports(WrapProcessInspectionReportQueryInfoDto dto, out int total)
    {
        var data = _wpRepo.All().AsNoTracking();

        if (!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End))
        {
            var begin = Convert.ToDateTime(dto.Begin);
            var end = Convert.ToDateTime(dto.End);
            data = data.Where(c => c.Time >= begin.Date &&
                                   c.Time <= end.Date);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = int.Parse(dto.TurnId);
            data = data.Where(c => c.TurnId == turnId);
        }

        if (!string.IsNullOrEmpty(dto.MachineId))
        {
            var machineId = int.Parse(dto.MachineId);
            data = data.Where(c => c.MachineId == machineId);
        }

        if (!string.IsNullOrEmpty(dto.State))
        {
            var result = int.Parse(dto.State);
            data = data.Where(c => c.Result == (QualityResult)result);
        }

        total = data.Count();

        var list = data.OrderByDescending(c => c.Time)
            .Skip(dto.Skip()).Take(dto.PageSize)
            .Select(c => new WrapProcessInspectionReportInfoDto()
            {
                Id = c.Id, Time = c.Time.ToString("yyyy-MM-dd"), SpecificationId = c.SpecificationId,
                SpecificationName = c.Specification.Name, TurnId = c.TurnId, TurnName = c.Turn.Name,
                MachineId = c.MachineId, MachineName = c.Machine.Name, OperatorName = c.OperatorName,
                Inspector = c.Inspector, Result = (int)c.Result, BatchNumber = c.BatchNumber,
                WeightLower = c.WeightLower, WeightUpper = c.WeightUpper,
                ResistanceLower = c.ResistanceLower, ResistanceUpper = c.ResistanceUpper,
                OtherIndicators = c.OtherIndicators, OtherCount = c.OtherCount, Score = c.Score,
                Remark = c.Remark, BatchUnqualified = c.BatchUnqualified
            }).ToList();

        var ids = list.Select(c => c.Id).ToList();
        var allDefects = _wpdRepo.All().AsNoTracking().Where(c => ids.Contains(c.ReportId))
            .Select(c => new BaseDefectInfoDto()
            {
                Id = c.Id, ReportId = c.ReportId, DefectId = c.DefectId, Count = c.Count
            }).ToList();

        foreach (var item in list)
        {
            var defects = allDefects.Where(c => c.ReportId == item.Id).ToList();
            item.Defects = defects;
        }

        return list;
    }

    public bool EditReport(WrapProcessInspectionReportInfoDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var report = new WrapProcessInspectionReport();
        var hasDefects = dto.Defects is { Count: > 0 };
        var result = true;
        var points = 0;

        if (modify)
        {
            report = _wpRepo.Get(dto.Id);
        }

        report.Time = Convert.ToDateTime(dto.Time);
        report.SpecificationId = dto.SpecificationId;
        report.TurnId = dto.TurnId;
        report.MachineId = dto.MachineId;
        report.BatchNumber = dto.BatchNumber;
        report.OperatorName = dto.OperatorName;
        report.WeightLower = dto.WeightLower;
        report.WeightUpper = dto.WeightUpper;
        report.ResistanceLower = dto.ResistanceLower;
        report.ResistanceUpper = dto.ResistanceUpper;
        report.OtherIndicators = dto.OtherIndicators;
        report.OtherCount = dto.OtherCount;
        report.Inspector = dto.Inspector;
        report.BatchUnqualified = dto.BatchUnqualified;
        report.Remark = dto.Remark;
        points += dto.WeightLower * 5;
        points += dto.WeightUpper * 5;
        points += dto.ResistanceLower * 3;
        points += dto.ResistanceUpper * 3;
        points += dto.OtherCount;

        if (hasDefects)
        {
            var defectIds = dto.Defects.Select(c => c.DefectId).ToList();
            var defects = _dRepo.All().Where(c => defectIds.Contains(c.Id)).ToList();
            foreach (var d in dto.Defects)
            {
                var currentDefect = defects.FirstOrDefault(c => c.Id == d.DefectId);
                if (currentDefect == null)
                    continue;

                points += Convert.ToInt32(d.Count * currentDefect.Score);
            }
        }
        
        report.Score = points;
        report.Result = points switch
        {
            <= 30 => QualityResult.Quality,
            < 100 and > 30 => QualityResult.Grade,
            >= 100 => QualityResult.Nonconforming,
        };

        if (modify)
            _wpRepo.Update(report);
        else
            _wpRepo.Add(report);

        result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;
        
        if (result)
        {
            if (modify)
            {
                var defects = _wpdRepo.All().Where(c => c.ReportId == report.Id).ToList();
                _wpdRepo.DeleteRange(defects);
            }

            if (hasDefects)
            {
                if (dto.Defects != null)
                {
                    var defectInfo = dto.Defects.Select(c => new WrapProcessInspectionReportDefect
                    {
                        ReportId = report.Id, DefectId = c.DefectId, Count = c.Count
                    }).ToList();
                    _wpdRepo.AddRange(defectInfo);
                }
            }
            
            result = _uow.Save() >= 0;
        }
        
        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";
        return result;
    }

    public bool RemoveReports(List<int> ids, out string message)
    {
        var reports = _wpRepo.All().Where(c => ids.Contains(c.Id)).ToList();
        _wpRepo.DeleteRange(reports);
        var result = _uow.Save() == ids.Count;
        message = result ? "删除成功" : "删除失败";
        return result;
    }

    public MemoryStream DownloadReports(WrapProcessInspectionReportQueryInfoDto dto)
    {
        throw new NotImplementedException();
    }
}