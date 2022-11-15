using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.WorkShop;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class WorkShopService : IWorkShopService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<WorkShop> _wsRepo;

    public WorkShopService(IRepository<WorkShop> wsRepo, IUnitOfWork uow)
    {
        _wsRepo = wsRepo;
        _uow = uow;
    }

    public IEnumerable<WorkShopTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        total = 0;
        var skip = (dto.PageNum - 1) * dto.PageSize;
        var data = _wsRepo.All().AsNoTracking();

        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = Convert.ToInt32(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.CreatedAtUtc).Skip(skip
        ).Take(dto.PageSize).Select(c => new WorkShopTableDto
        {
            Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled
        }).ToList();

        return result;
    }

    public bool Add(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_wsRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该车间名称已被使用, 请使用其他名称";
            return false;
        }

        var ws = new WorkShop
        {
            Name = dto.Name,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _wsRepo.Add(ws);

        return _uow.Save() > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_wsRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = "该车间名称已被使用, 请使用其他名称";
            return false;
        }

        var ws = _wsRepo.Get(dto.Id);
        ws.Name = dto.Name;
        ws.Status = dto.State ? Status.Enabled : Status.Disabled;

        _wsRepo.Update(ws);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions(bool stringValue)
    {
        var data = _wsRepo.All().Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
        {
            Value = stringValue ? c.Name : c.Id, Text = c.Name
        }).ToList();

        return data;
    }
}