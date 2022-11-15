using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.MachineModel;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class MachineModelService : IMachineModelService
{
    private readonly IRepository<MachineModel> _mmRepo;
    private readonly IUnitOfWork _uow;

    public MachineModelService(IRepository<MachineModel> mmRepo, IUnitOfWork uow)
    {
        _mmRepo = mmRepo;
        _uow = uow;
    }

    public IEnumerable<MachineModelTableDto> GetTable(MachineModelQueryInfoDto dto, out int total)
    {
        total = 0;
        // var skip = (dto.PageNum - 1) * dto.PageSize;
        var data = _mmRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.WorkShopId))
        {
            var wsId = int.Parse(dto.WorkShopId);
            data = data.Where(c => c.WorkShopId == wsId);
        }

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = Convert.ToInt32(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.Include(c => c.WorkShop).Include(c => c.Model).OrderByDescending(c => c.CreatedAtUtc)
            .Skip(dto.Skip())
            .Take(dto.PageSize).Select(c => new MachineModelTableDto
            {
                Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled, WorkShopId = c.WorkShopId,
                WorkShopName = c.WorkShop.Name,
                ModelId = c.ModelId
                // , ModelName = c.ModelId == 0 ? "" : c.Model.Name
            }).ToList();

        return result;
    }

    public bool Add(MachineModelEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_mmRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该机型名称存在, 请使用其他名称";
            return false;
        }

        var machineModel = new MachineModel
        {
            Name = dto.Name,
            WorkShopId = dto.WorkShopId,
            ModelId = dto.ModelId,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _mmRepo.Add(machineModel);

        return _uow.Save() > 0;
    }

    public bool Update(MachineModelEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_mmRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = "该机型名称已被使用, 请使用其他名称";
            return false;
        }

        var machineModel = _mmRepo.Get(dto.Id);
        machineModel.Name = dto.Name;
        machineModel.WorkShopId = dto.WorkShopId;
        machineModel.ModelId = dto.ModelId;
        machineModel.Status = dto.State ? Status.Enabled : Status.Disabled;

        _mmRepo.Update(machineModel);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _mmRepo.All().AsNoTracking().OrderBy(c => c.Name).Where(c => c.Status == Status.Enabled).Select(c =>
            new BaseOptionDto
            {
                Value = c.Id, Text = c.Name
            }).ToList();

        return data;
    }
}