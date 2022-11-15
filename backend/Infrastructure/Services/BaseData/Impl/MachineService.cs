using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;
using Infrastructure.Services.System;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class MachineService : IMachineService
{
    private readonly ILogService _logService;
    private readonly IRepository<Machine> _machineRepo;
    private readonly IUnitOfWork _uow;

    public MachineService(IRepository<Machine> machineRepo, ILogService logService, IUnitOfWork uow)
    {
        _machineRepo = machineRepo;
        _logService = logService;
        _uow = uow;
    }

    public IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        total = 0;
        var data = _machineRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new BaseTableDto
                {
                    Id = c.Id, Name = c.Name,
                    State = c.Status == Status.Enabled
                }).ToList();

        return result;
    }

    public bool Add(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_machineRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该机台名称已被使用, 请使用其他名称";
            return false;
        }

        var machine = new Machine
        {
            Name = dto.Name.Trim(),
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        // _logService.AddLog($"添加机台 {dto.Name}");

        _machineRepo.Add(machine);

        return _uow.Save() > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_machineRepo.All()
            .Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = "该机台名称已被使用, 请使用其他名称";
            return false;
        }

        var machine = _machineRepo.Get(dto.Id);
        machine.Name = dto.Name.Trim();
        machine.Status = dto.State ? Status.Enabled : Status.Disabled;

        // _logService.AddLog($"编辑机台 {dto.Id}, 机台名称为 {dto.Name}");

        _machineRepo.Update(machine);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _machineRepo.All().AsNoTracking().OrderBy(c => c.Name)
            .Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
            {
                Value = c.Id, Text = c.Name
            }).ToList();

        return data;
    }
}