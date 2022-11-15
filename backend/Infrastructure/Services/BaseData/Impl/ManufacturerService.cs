using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class ManufacturerService : IManufacturerService
{
    private readonly IRepository<Manufacturer> _mfRepo;
    private readonly IUnitOfWork _uow;

    public ManufacturerService(IRepository<Manufacturer> mfRepo, IUnitOfWork uow)
    {
        _mfRepo = mfRepo;
        _uow = uow;
    }

    public IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _mfRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = Convert.ToInt32(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
            new BaseTableDto
            {
                Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled
            }).ToList();

        return result;
    }

    public bool Add(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_mfRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该名称已被使用, 请使用其他名称";
            return false;
        }

        var manufacturer = new Manufacturer
        {
            Name = dto.Name,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _mfRepo.Add(manufacturer);

        return _uow.Save() > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_mfRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = "该名称已被使用, 请使用其他名称";
            return false;
        }

        var manufacturer = _mfRepo.Get(dto.Id);
        if (manufacturer == null)
        {
            failReason = "没有找到对应的数据";
            return false;
        }

        manufacturer.Name = dto.Name;
        manufacturer.Status = dto.State ? Status.Enabled : Status.Disabled;

        _mfRepo.Update(manufacturer);

        return _uow.Save() >= 0;
    }

    public bool Delete(IEnumerable<int> ids)
    {
        var manufacturers = _mfRepo.All().Where(c => ids.Contains(c.Id)).ToList();

        _mfRepo.DeleteRange(manufacturers);

        return _uow.Save() > 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _mfRepo.All().Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
        {
            Value = c.Id, Text = c.Name
        }).ToList();

        return data;
    }
}