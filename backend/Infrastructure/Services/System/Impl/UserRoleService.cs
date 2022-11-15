using System.Collections.Generic;
using System.Linq;
using Core.Dtos.UserRole;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Services.System.Impl;

public class UserRoleService : IUserRoleService
{
    private readonly IRepository<Role> _rRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<UserRole> _urRepo;

    public UserRoleService(IRepository<UserRole> urRepo, IRepository<Role> rRepo, IUnitOfWork uow)
    {
        _urRepo = urRepo;
        _rRepo = rRepo;
        _uow = uow;
    }

    public UserRoleEditDto Get(int id)
    {
        var dto = new UserRoleEditDto();
        if (_urRepo.All().Any(c => c.RoleId == id))
        {
            var userRole = _urRepo.All().Include(c => c.Role).FirstOrDefault(c => c.RoleId == id);
            dto.Id = userRole.Id;
            dto.RoleName = userRole.Role.Name;
            dto.RoleId = userRole.RoleId;
            dto.Users = JsonConvert.DeserializeObject<List<int>>(userRole.UserIds);
        }
        else
        {
            var role = _rRepo.Get(id);
            dto.RoleId = id;
            dto.RoleName = role.Name;
        }

        return dto;
    }

    public bool Add(UserRoleEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        var userRole = new UserRole
        {
            RoleId = dto.RoleId,
            UserIds = JsonConvert.SerializeObject(dto.Users)
        };

        _urRepo.Add(userRole);

        return _uow.Save() > 0;
    }

    public bool Update(UserRoleEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        var userRole = _urRepo.Get(dto.Id);
        userRole.RoleId = dto.RoleId;
        userRole.UserIds = JsonConvert.SerializeObject(dto.Users);

        _urRepo.Update(userRole);

        return _uow.Save() >= 0;
    }

    public UserRole GetRoleIdByUserId(int userId)
    {
        var role = _urRepo.All().Include(c => c.Role).FirstOrDefault(c => c.UserIds.Contains(userId.ToString()));
        return role;
    }

    /// <summary>
    ///     获取用户所有角色Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public List<int> GetRoleIdsByUserId(int userId)
    {
        var roleIds = _urRepo.All().Where(c => c.UserIds.Contains(userId.ToString())).Select(c => c.RoleId).ToList();
        return roleIds;
    }
}