using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class TurnService : ITurnService
{
    private readonly IRepository<Turn> _turnRepo;
    private readonly IUnitOfWork _uow;

    public TurnService(IRepository<Turn> turnRepo, IUnitOfWork uow)
    {
        _turnRepo = turnRepo;
        _uow = uow;
    }

    public IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        total = 0;
        var skip = (dto.PageNum - 1) * dto.PageSize;
        var data = _turnRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = Convert.ToInt32(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc).Skip(skip)
            .Take(dto.PageSize).Select(c => new BaseTableDto
            {
                Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled
            }).ToList();

        return result;
    }

    public bool Add(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_turnRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该班次名称已存在, 请使用其他名称";
            return false;
        }

        var turn = new Turn
        {
            Name = dto.Name,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _turnRepo.Add(turn);

        return _uow.Save() > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_turnRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = "该班次名称已存在, 请使用其他名称";
            return false;
        }

        var turn = _turnRepo.Get(dto.Id);
        turn.Name = dto.Name;
        turn.Status = dto.State ? Status.Enabled : Status.Disabled;

        _turnRepo.Update(turn);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _turnRepo.All().AsNoTracking().OrderBy(c => c.Name)
            .Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
            {
                Value = c.Id, Text = c.Name
            }).ToList();
        return data;
    }
}