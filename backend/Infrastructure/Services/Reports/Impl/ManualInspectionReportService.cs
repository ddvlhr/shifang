using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.ManualInspectionReport;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Reports.Impl;

[AutoInject(typeof(IManualInspectionReportService), InjectType.Scope)]
public class ManualInspectionReportService : IManualInspectionReportService
{
    private readonly IRepository<Core.Entities.Defect> _defectRepo;
    private readonly IRepository<ManualInspectionReportDefect> _midRepo;
    private readonly IRepository<ManualInspectionReport> _miRepo;
    private readonly IUnitOfWork _uow;

    public ManualInspectionReportService(IRepository<ManualInspectionReport> miRepo,
        IRepository<ManualInspectionReportDefect> midRepo,
        IUnitOfWork uow,
        IRepository<Core.Entities.Defect> defectRepo)
    {
        _miRepo = miRepo;
        _midRepo = midRepo;
        _uow = uow;
        _defectRepo = defectRepo;
    }

    public IEnumerable<ManualInspectionReportInfoDto> GetReports(ManualInspectionReportQueryInfoDto dto, out int total)
    {
        var data = _miRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End))
        {
            var begin = Convert.ToDateTime(dto.Begin);
            var end = Convert.ToDateTime(dto.End);
            data = data.Where(c => c.Time.Date >= begin && c.Time.Date <= end);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.Result))
        {
            var result = int.Parse(dto.Result);
            data = data.Where(c => c.Result == (QualityResult)result);
        }

        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c => c.Operation.Contains(dto.Query) ||
                                   c.Specification.Name.Contains(dto.Query));

        total = data.Count();

        var list = data.OrderByDescending(c => c.Time).Skip(dto.Skip())
            .Take(dto.PageSize).Select(c => new ManualInspectionReportInfoDto
            {
                Id = c.Id, Time = c.Time.ToString("yyyy-MM-dd HH:mm:ss"), SpecificationId = c.SpecificationId,
                SpecificationName = c.Specification.Name, Count = c.Count, Result = (int)c.Result,
                Operation = c.Operation
            }).ToList();

        var reportIds = list.Select(c => c.Id).ToList();
        var defects = _midRepo.All().Where(c => reportIds.Contains(c.ReportId)).ToList();
        foreach (var item in list)
            item.DefectInfo = defects.Where(c => c.ReportId == item.Id).Select(c =>
                new ManualInspectionReportDefectInfoDto
                {
                    Id = c.Id, DefectId = c.DefectId, Count = c.Count
                }).ToList();

        return list;
    }

    public bool Edit(ManualInspectionReportInfoDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var report = new ManualInspectionReport();
        var result = true;
        var hasDefects = dto.DefectInfo == null || dto.DefectInfo.Count > 0;
        if (modify)
            report = _miRepo.Get(dto.Id);

        report.Time = Convert.ToDateTime(dto.Time);
        report.SpecificationId = dto.SpecificationId;
        report.Operation = dto.Operation;
        report.Count = dto.Count;

        if (!hasDefects)
        {
            report.Result = QualityResult.Quality;
        }
        else
        {
            var defectIds = dto.DefectInfo.Select(c => c.DefectId).ToList();
            var defects = _defectRepo.All().Where(c => defectIds.Contains(c.Id)).ToList();
            if (defects.Count == 0)
                report.Result = QualityResult.Quality;
            else if (defects.Any(c => c.DefectCategory > DefectCategory.B))
                report.Result = QualityResult.Grade;
            else
                report.Result = QualityResult.Nonconforming;
        }

        if (modify)
            _miRepo.Update(report);
        else
            _miRepo.Add(report);

        result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;

        if (result)
        {
            if (modify)
            {
                var existDefects = _midRepo.All().Where(c => c.ReportId == report.Id).ToList();
                _midRepo.DeleteRange(existDefects);
            }

            if (hasDefects)
            {
                var defectInfo = dto.DefectInfo.Select(c => new ManualInspectionReportDefect
                {
                    ReportId = report.Id, DefectId = c.DefectId, Count = c.Count
                }).ToList();

                _midRepo.AddRange(defectInfo);
                result = _uow.Save() > 0;
            }
        }

        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";
        return result;
    }

    public bool Remove(List<int> ids, out string message)
    {
        var reports = _miRepo.All().Where(c => ids.Contains(c.Id)).ToList();
        _miRepo.DeleteRange(reports);
        var result = _uow.Save() >= ids.Count;
        message = result ? "删除成功" : "删除失败";
        return result;
    }
}