using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Dtos.Report;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Reports.Impl;

[AutoInject(typeof(IProcessDisciplineReportService), InjectType.Scope)]
public class ProcessDisciplineReportService: IProcessDisciplineReportService
{
    private readonly IRepository<ProcessDisciplineReport> _pdrRepo;
    private readonly IUnitOfWork _uow;

    public ProcessDisciplineReportService(IRepository<ProcessDisciplineReport> pdrRepo,
        IUnitOfWork uow)
    {
        _pdrRepo = pdrRepo;
        _uow = uow;
    }
    public IEnumerable<ProcessDisciplineReportInfoDto> GetTable(ProcessDisciplineReportQueryInfoDto dto, out int total)
    {
        var data = _pdrRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query))
        {
            data = data.Where(c => c.Department.Name.Contains(dto.Query) ||
                                   c.Description.Contains(dto.Query));
        }

        if (!string.IsNullOrEmpty(dto.Department))
        {
            var departmentId = int.Parse(dto.Department);
            data = data.Where(c => c.DepartmentId == departmentId);
        }

        total = data.Count();

        var result = data.OrderByDescending(c => c.Time)
            .Skip(dto.Skip()).Take(dto.PageSize)
            .Select(c => new ProcessDisciplineReportInfoDto()
            {
                Id = c.Id, Time = c.Time.ToString("yyyy-MM-dd"),
                DepartmentId = c.DepartmentId, DepartmentName = c.Department.Name,
                Description = c.Description, Reward = c.Reward, Punishment = c.Punishment
            }).ToList();

        return result;
    }

    public bool Edit(ProcessDisciplineReportInfoDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var report = new ProcessDisciplineReport();
        if (modify)
        {
            report = _pdrRepo.Get(dto.Id);
        }

        report.Time = Convert.ToDateTime(dto.Time);
        report.DepartmentId = dto.DepartmentId;
        report.Description = dto.Description.Trim();
        report.Reward = dto.Reward?.Trim();
        report.Punishment = dto.Punishment?.Trim();
        
        if (modify)
            _pdrRepo.Update(report);
        else
            _pdrRepo.Add(report);

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;
        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";

        return result;
    }

    public bool Remove(List<int> ids, out string message)
    {
        var reports = _pdrRepo.All().Where(c => ids.Contains(c.Id)).ToList();
        if (reports.Count > 0)
        {
            _pdrRepo.DeleteRange(reports);
        }

        var result = _uow.Save() > 0;
        message = result ? "删除成功" : "删除失败";
        return result;
    }

    public MemoryStream Download(ProcessDisciplineReportQueryInfoDto dto)
    {
        throw new NotImplementedException();
    }
}