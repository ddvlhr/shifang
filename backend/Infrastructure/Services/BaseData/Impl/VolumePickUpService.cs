using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

[AutoInject(typeof(IVolumePickUpService), InjectType.Scope)]
public class VolumePickUpService: IVolumePickUpService
{
    private readonly IRepository<VolumePickUp> _vpRepo;
    private readonly IUnitOfWork _uow;

    public VolumePickUpService(IRepository<VolumePickUp> vpRepo,
        IUnitOfWork uow)
    {
        _vpRepo = vpRepo;
        _uow = uow;
    }
    public IEnumerable<BaseTableDto> GetVolumePickUps(BaseQueryInfoDto dto, out int total)
    {
        var data = _vpRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query))
        {
            data = data.Where(c => c.Name.Contains(dto.Query));
        }

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();

        var result = data.OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
            new BaseTableDto()
            {
                Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled
            }).ToList();

        return result;
    }

    public bool Edit(BaseEditDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var volumePickUp = new VolumePickUp();
        if (modify)
        {
            if (_vpRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
            {
                message = "该卷接机已存在, 请使用其他名称";
                return false;
            }

            volumePickUp = _vpRepo.Get(dto.Id);
        }
        else
        {
            if (_vpRepo.All().Any(c => c.Name == dto.Name))
            {
                message = "该卷接机已存在, 请使用其他名称";
                return false;
            }
        }

        volumePickUp.Name = dto.Name;
        volumePickUp.Status = dto.State ? Status.Enabled : Status.Disabled;
        
        if (modify)
            _vpRepo.Update(volumePickUp);
        else
            _vpRepo.Add(volumePickUp);

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;
        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";
        return result;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var options = _vpRepo.All().OrderByDescending(c => c.ModifiedAtUtc).Where(c => c.Status == Status.Enabled)
            .Select(c => new BaseOptionDto()
            {
                Value = c.Id, Text = c.Name
            }).ToList();

        return options;
    }
}