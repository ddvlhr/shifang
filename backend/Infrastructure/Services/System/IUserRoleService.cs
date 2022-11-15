using System.Collections.Generic;
using Core.Dtos.UserRole;
using Core.Entities;

namespace Infrastructure.Services.System;

public interface IUserRoleService
{
    UserRoleEditDto Get(int id);
    bool Add(UserRoleEditDto dto, out string failReason);
    bool Update(UserRoleEditDto dto, out string failReason);
    UserRole GetRoleIdByUserId(int userId);
    List<int> GetRoleIdsByUserId(int userId);
}