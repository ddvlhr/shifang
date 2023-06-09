using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Role;
using Core.Entities;
using Core.Enums;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Services.System.Impl;

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roleRepo;
    private readonly IUnitOfWork _uow;

    public RoleService(IRepository<Role> roleRepo, IUnitOfWork uow)
    {
        _roleRepo = roleRepo;
        _uow = uow;
    }

    public IEnumerable<RoleTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _roleRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new RoleTableDto
                {
                    Id = c.Id, Name = c.Name, RoleMenu = JsonConvert.DeserializeObject<List<int>>(c.RoleMenu),
                    CanSeeOtherData = c.CanSeeOtherData,
                    State = c.Status == Status.Enabled, EquipmentType = (int)c.EquipmentType
                }).ToList();

        return result;
    }

    public bool Add(RoleEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_roleRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该角色名称已存在, 请使用其他名称";
            return false;
        }

        var role = new Role
        {
            Name = dto.Name,
            RoleMenu = JsonConvert.SerializeObject(dto.RoleMenu),
            CanSeeOtherData = dto.CanSeeOtherData,
            Status = dto.State ? Status.Enabled : Status.Disabled,
            EquipmentType = (DepartmentType) dto.EquipmentType
        };

        _roleRepo.Add(role);

        return _uow.Save() > 0;
    }

    public bool Update(RoleEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_roleRepo.All().Any(c => c.Name == dto.Name && c.Id != dto.Id))
        {
            failReason = "该角色名称已存在, 请使用其他名称";
            return false;
        }

        var role = _roleRepo.Get(dto.Id);
        role.Name = dto.Name;
        role.RoleMenu = JsonConvert.SerializeObject(dto.RoleMenu);
        role.CanSeeOtherData = dto.CanSeeOtherData;
        role.Status = dto.State ? Status.Enabled : Status.Disabled;
        role.EquipmentType = (DepartmentType)dto.EquipmentType;

        _roleRepo.Update(role);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _roleRepo.All().AsNoTracking().Where(c => c.Status == Status.Enabled).Select(c =>
            new BaseOptionDto
            {
                Value = c.Id, Text = c.Name
            }).ToList();

        return data;
    }
}