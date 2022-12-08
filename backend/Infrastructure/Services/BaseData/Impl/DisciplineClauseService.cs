using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Models;
using Core.SugarEntities;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using SqlSugar;

namespace Infrastructure.Services.BaseData.Impl;

[AutoInject(typeof(IDisciplineClauseService), InjectType.Scope)]
public class DisciplineClauseService : SugarRepository<DisciplineClause>, IDisciplineClauseService
{
    public async Task<ResultViewModel<BaseEditDto>> EditAsync(BaseEditDto dto)
    {
        var result = new ResultViewModel<BaseEditDto>();
        var item = new DisciplineClause();
        var modify = dto.Id > 0;
        if (modify)
        {
            if (await base.IsAnyAsync(c => c.Id != dto.Id && c.Name == dto.Name))
            {
                result.Success = false;
                result.Message = "该名称已被使用";
                return result;
            }
            item = await base.GetSingleAsync(it => it.Id == dto.Id);
        }
        else
        {
            if (await base.IsAnyAsync(c => c.Name == dto.Name))
            {
                result.Success = false;
                result.Message = "该名称已被使用";
                return result;
            }
        }

        item.Name = dto.Name;
        item.Status = dto.State ? Status.Enabled : Status.Disabled;
        if (!modify)
            item.CreatedAtUtc = DateTime.Now;
        item.ModifiedAtUtc = DateTime.Now;

        var rows = await base.Context.Storageable<DisciplineClause>(item).ExecuteCommandAsync();

        var ret = modify ? rows >= 0 : rows > 0;
        result.Success = ret;
        result.Message = modify ? (ret ? "修改成功" : "修改失败") : (ret ? "添加成功" : "添加失败");

        return result;
    }

    public async Task<ResultViewModel<BaseEditDto>> GetByIdAsync(int id)
    {
        var result = new ResultViewModel<BaseEditDto>();
        var item = await base.GetSingleAsync(it => it.Id == id);
        if (item == null)
        {
            result.Success = false;
            result.Message = "没有找到对应的数据";
            return result;
        }

        result.Success = true;
        result.Data = new BaseEditDto
        {
            Id = item.Id,
            Name = item.Name,
            State = item.Status == Status.Enabled
        };

        return result;
    }

    public async Task<IEnumerable<BaseOptionDto>> GetOptionsAsync()
    {
        var options = await base.Context.Queryable<DisciplineClause>()
            .Where(it => it.Status == Status.Enabled)
            .Select(it => new BaseOptionDto
            {
                Value = it.Id,
                Text = it.Name
            })
            .ToListAsync();

        return options;
    }

    public async Task<PageViewModel<BaseTableDto>> GetTableAsync(BaseQueryInfoDto dto)
    {
        RefAsync<int> total = 0;
        var pageViewModel = new PageViewModel<BaseTableDto>();
        var filter = Expressionable.Create<DisciplineClause>()
            .AndIF(!string.IsNullOrEmpty(dto.Query), it => it.Name.Contains(dto.Query))
            .AndIF(!string.IsNullOrEmpty(dto.State), it => it.Status == (Status)int.Parse(dto.State))
            .ToExpression();

        var list = await base.Context.Queryable<DisciplineClause>()
        .Where(filter)
            .OrderByDescending(it => it.ModifiedAtUtc)
            .Select(c => new BaseTableDto
            {
                Id = c.Id,
                Name = c.Name,
                State = c.Status == Status.Enabled
            })
            .ToPageListAsync(dto.PageNum, dto.PageSize, total);

        pageViewModel.Total = total;
        pageViewModel.List = list;

        return pageViewModel;
    }
}