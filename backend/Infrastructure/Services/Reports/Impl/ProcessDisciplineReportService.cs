using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Dtos.Report;
using Core.SugarEntities;
using Core.Enums;
using Core.Models;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using SqlSugar;

namespace Infrastructure.Services.Reports.Impl;

[AutoInject(typeof(IProcessDisciplineReportService), InjectType.Scope)]
public class ProcessDisciplineReportService : SugarRepository<Core.SugarEntities.ProcessDisciplineReport>, IProcessDisciplineReportService
{
    public async Task<PageViewModel<ProcessDisciplineReportInfoDto>> GetTableAsync(ProcessDisciplineReportQueryInfoDto dto)
    {
        RefAsync<int> total = 0;
        var result = new PageViewModel<ProcessDisciplineReportInfoDto>();
        var filter = Expressionable.Create<Core.SugarEntities.ProcessDisciplineReport>()
            .AndIF(!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End),
                c => c.Time.Date >= Convert.ToDateTime(dto.Begin) && c.Time.Date <= Convert.ToDateTime(dto.End))
            .AndIF(!string.IsNullOrEmpty(dto.Query), c => c.Department.Name.Contains(dto.Query) ||
                                                          c.DisciplineClass.Name.Contains(dto.Query) ||
                                                          c.DisciplineClause.Name.Contains(dto.Query) ||
                                                          c.Description.Contains(dto.Query))
            .AndIF(!string.IsNullOrEmpty(dto.Department), c => c.DepartmentId == int.Parse(dto.Department))
            .AndIF(!string.IsNullOrEmpty(dto.ClassId), c => c.ClassId == int.Parse(dto.ClassId))
            .AndIF(!string.IsNullOrEmpty(dto.ClauseId), c => c.ClauseId == int.Parse(dto.ClauseId))
            .ToExpression();

        var list = await base.Context.Queryable<Core.SugarEntities.ProcessDisciplineReport>()
            .LeftJoin<Department>((c, d) => c.DepartmentId == d.Id)
            .LeftJoin<DisciplineClass>((c, d, dc) => c.ClassId == dc.Id)
            .LeftJoin<DisciplineClause>((c, d, dc, cl) => c.ClauseId == cl.Id)
            .Where(filter)
            .OrderByDescending(c => c.ModifiedAtUtc)
            .Select(c => new ProcessDisciplineReportInfoDto()
            {
                Id = c.Id,
                Time = c.Time.ToString("yyyy-MM-dd"),
                ClassId = c.ClassId,
                ClassName = c.DisciplineClass.Name,
                ClauseId = c.ClauseId,
                ClauseName = c.DisciplineClause.Name,
                DepartmentId = c.DepartmentId,
                DepartmentName = c.Department.Name,
                Description = c.Description,
                Reward = c.Reward,
                Punishment = c.Punishment
            }).ToPageListAsync(dto.PageNum, dto.PageSize, total);

        result.Total = total;
        result.List = list;

        return result;
    }

    public async Task<ResultViewModel<ProcessDisciplineReportInfoDto>> GetByIdAsync(int id)
    {
        var result = new ResultViewModel<ProcessDisciplineReportInfoDto>();
        var item = await base.Context.Queryable<ProcessDisciplineReport>()
            .LeftJoin<Department>((c, d) => c.DepartmentId == d.Id)
            .LeftJoin<DisciplineClass>((c, d, dc) => c.ClassId == dc.Id)
            .LeftJoin<DisciplineClause>((c, d, dc, cl) => c.ClauseId == cl.Id)
            .FirstAsync(c => c.Id == id);
        if (item == null)
        {
            result.Success = false;
            result.Message = "未找到该记录";
            return result;
        }

        result.Success = true;
        result.Data = new ProcessDisciplineReportInfoDto
        {
            Id = item.Id,
            Time = item.Time.ToString("yyyy-MM-dd"),
            ClassId = item.ClassId,
            ClauseId = item.ClauseId,
            DepartmentId = item.DepartmentId,
            Description = item.Description,
            Reward = item.Reward,
            Punishment = item.Punishment
        };
        return result;
    }

    public async Task<ResultViewModel<ProcessDisciplineReportInfoDto>> EditAsync(ProcessDisciplineReportInfoDto dto)
    {
        var modify = dto.Id > 0;
        var result = new ResultViewModel<ProcessDisciplineReportInfoDto>();
        var report = new Core.SugarEntities.ProcessDisciplineReport();
        if (modify)
        {
            report = await base.GetSingleAsync(c => c.Id == dto.Id);
        }

        report.Time = Convert.ToDateTime(dto.Time);
        report.DepartmentId = dto.DepartmentId;
        report.ClassId = dto.ClassId;
        report.ClauseId = dto.ClauseId;
        report.Description = dto.Description;
        report.Reward = dto.Reward;
        report.Punishment = dto.Punishment;
        if (!modify)
            report.CreatedAtUtc = DateTime.Now;
        report.ModifiedAtUtc = DateTime.Now;

        var rows = await base.Context.Storageable<Core.SugarEntities.ProcessDisciplineReport>(report).ExecuteCommandAsync();
        var ret = modify ? rows >= 0 : rows > 0;
        result.Success = ret;
        result.Message = ret ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";

        return result;
    }

    public async Task<ResultViewModel<Core.SugarEntities.ProcessDisciplineReport>> RemoveAsync(List<int> ids)
    {
        var result = new ResultViewModel<Core.SugarEntities.ProcessDisciplineReport>();
        var items = await base.Context.Queryable<Core.SugarEntities.ProcessDisciplineReport>().In(ids).ToListAsync();
        if (items == null || items.Count == 0)
        {
            result.Success = false;
            result.Message = "未找到该记录";
            return result;
        }

        var rows = await base.Context.Deleteable<Core.SugarEntities.ProcessDisciplineReport>().In(ids).ExecuteCommandAsync();

        result.Success = rows > 0;
        result.Message = rows > 0 ? "删除成功" : "删除失败";
        return result;
    }

    public Task<MemoryStream> DownloadAsync(ProcessDisciplineReportQueryInfoDto dto)
    {
        throw new NotImplementedException();
    }
}