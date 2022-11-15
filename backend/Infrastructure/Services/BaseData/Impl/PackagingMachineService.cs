using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using NotImplementedException = System.NotImplementedException;

namespace Infrastructure.Services.BaseData.Impl;

[AutoInject(typeof(IPackagingMachineService), InjectType.Scope)]
public class PackagingMachineService: IPackagingMachineService
{
    private readonly IRepository<PackagingMachine> _pmRepo;
    private readonly IUnitOfWork _uow;

    public PackagingMachineService(IRepository<PackagingMachine> pmRepo,
        IUnitOfWork uow)
    {
        _pmRepo = pmRepo;
        _uow = uow;
    }
    public IEnumerable<BaseTableDto> GetPackagingMachines(BaseQueryInfoDto dto, out int total)
    {
        var data = _pmRepo.All().AsNoTracking();
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

        var result = data.OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip())
            .Take(dto.PageSize).Select(c => new BaseTableDto()
            {
                Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled
            }).ToList();

        return result;
    }

    public bool Edit(BaseEditDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var packagingMachine = new PackagingMachine();
        if (modify)
        {
            if (_pmRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
            {
                message = "该包装机已存在, 请使用其他名称";
                return false;
            }

            packagingMachine = _pmRepo.Get(dto.Id);
        }
        else
        {
            if (_pmRepo.All().Any(c => c.Name == dto.Name))
            {
                message = "该包装机已存在, 请使用其他名称";
                return false;
            }
        }

        packagingMachine.Name = dto.Name;
        packagingMachine.Status = dto.State ? Status.Enabled : Status.Disabled;
        
        if (modify)
            _pmRepo.Update(packagingMachine);
        else
            _pmRepo.Add(packagingMachine);

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;

        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";

        return result;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var options = _pmRepo.All().Where(c => c.Status == Status.Enabled)
            .OrderByDescending(c => c.ModifiedAtUtc).Select(c => new BaseOptionDto()
            {
                Value = c.Id, Text = c.Name
            }).ToList();

        return options;
    }
}