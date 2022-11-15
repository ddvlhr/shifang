using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Authorize;
using Core.Dtos.User;
using Core.Entities;

namespace Infrastructure.Services.System;

public interface IUserService
{
    bool IsValid(LoginUserDto dto, out UserInfoDto userInfo, out string failReason);
    User GetUserInfo(string userName);
    IEnumerable<UserTableDto> GetUserTable(BaseQueryInfoDto dto, out int total);
    bool AddUser(EditUserDto dto, out string failReason);
    bool EditUser(EditUserDto dto, out string failReason);
    bool ModifyPassword(ModifyPasswordDto dto, out string failReason);
    bool ResetPassword(int id, out string failReason);
    string GetUserName(int id);
    IEnumerable<BaseOptionDto> GetOptions();
    IEnumerable<BaseOptionDto> GetNoRoleUsers();
}