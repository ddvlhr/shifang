using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Authorize;
using Core.Dtos.User;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Services.System.Impl;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IRepository<Role> _rRepo;
    private readonly Settings _setting;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<UserRole> _urRepo;
    private readonly IRepository<User> _userRepo;

    public UserService(IRepository<User> userRepo, IRepository<UserRole> urRepo, IRepository<Role> rRepo,
        IUnitOfWork uow,
        IHttpContextAccessor accessor, IOptionsSnapshot<Settings> settings)
    {
        _userRepo = userRepo;
        _urRepo = urRepo;
        _rRepo = rRepo;
        _uow = uow;
        _accessor = accessor;
        _setting = settings.Value;
    }

    public bool IsValid(LoginUserDto dto, out UserInfoDto userInfo,
        out string failReason)
    {
        failReason = "";
        userInfo = new UserInfoDto();
        var user = GetUserInfo(dto.UserName);
        if (user == null)
        {
            failReason = "没有找到该账号";
            return false;
        }

        if (user.HashedPassword != Encrypt.DesEncrypt(dto.Password))
        {
            failReason = "账号密码错误";
            return false;
        }

        if (user.Status != Status.Disabled)
        {
            var userRole = _urRepo.All().FirstOrDefault(c => c.UserIds.Contains(user.Id.ToString()));
            if (userRole == null)
            {
                failReason = "该用户还没有分配角色, 无法使用系统, 请联系管理员设置角色.";
                return false;
            }

            userInfo.Id = user.Id;
            userInfo.UserName = user.UserName;
            userInfo.NickName = user.NickName;
            userInfo.RoleId = userRole.RoleId;
            var role = _rRepo.Get(userRole.RoleId);
            userInfo.CanSeeOtherData = role.CanSeeOtherData;
            if (role.Id == _setting.AdminTypeId) userInfo.ShowSettings = true;
            return true;
        }

        failReason = "该账号已被停用";
        return false;
    }

    public User GetUserInfo(string userName)
    {
        return _userRepo.All()
            .FirstOrDefault(c => c.UserName == userName);
    }

    public IEnumerable<UserTableDto> GetUserTable(BaseQueryInfoDto dto, out int total)
    {
        total = 0;
        var skip = (dto.PageNum - 1) * dto.PageSize;
        var users = _userRepo.All();
        if (!string.IsNullOrEmpty(dto.Query))
            users = users.Where(c =>
                c.UserName.Contains(dto.Query) ||
                c.NickName.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            users = users.Where(c => c.Status == (Status)state);
        }

        total = users.Count();
        var result = users.Select(c => new UserTableDto
        {
            Id = c.Id,
            UserName = c.UserName,
            NickName = c.NickName,
            State = c.Status == Status.Enabled
        }).Skip(skip).Take(dto.PageSize).ToList();
        return result;
    }

    public bool AddUser(EditUserDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_userRepo.All().Any(c => c.UserName == dto.UserName))
        {
            failReason = "该用户名已存在, 请使用其他用户名";
            return false;
        }

        var user = new User
        {
            UserName = dto.UserName,
            NickName = dto.NickName,
            HashedPassword = Encrypt.DesEncrypt("123456"),
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _userRepo.Add(user);

        return _uow.Save() > 0;
    }

    public bool EditUser(EditUserDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_userRepo.All()
            .Any(c => c.UserName == dto.UserName && c.Id != dto.Id))
        {
            failReason = "该用户名已存在, 请使用其他用户名";
            return false;
        }

        var user = _userRepo.Get(dto.Id);
        user.UserName = dto.UserName;
        user.NickName = dto.NickName;
        user.Status = dto.State ? Status.Enabled : Status.Disabled;

        _userRepo.Update(user);

        return _uow.Save() >= 0;
    }

    public bool ModifyPassword(ModifyPasswordDto dto, out string failReason)
    {
        failReason = string.Empty;
        var userId = _accessor.HttpContext.getUserId();
        if (userId != dto.Id)
        {
            failReason = "不允许非本人修改密码, 请登录账号后再修改密码";
            return false;
        }

        var user = _userRepo.Get(dto.Id);
        if (Encrypt.DesDecrypt(user.HashedPassword) != dto.Origin)
        {
            failReason = "原密码不一致，请输入正确的原密码";
            return false;
        }

        user.HashedPassword = Encrypt.DesEncrypt(dto.New);

        _userRepo.Update(user);

        return _uow.Save() >= 0;
    }

    public bool ResetPassword(int id, out string failReason)
    {
        failReason = string.Empty;
        var roleId = _accessor.HttpContext.getUserRoleId();
        if (roleId != _setting.AdminTypeId)
        {
            failReason = "没有权限重置密码, 请联系管理员重置密码";
            return false;
        }

        var user = _userRepo.Get(id);
        user.HashedPassword = Encrypt.DesEncrypt("123456");
        _userRepo.Update(user);
        return _uow.Save() >= 0;
    }

    public string GetUserName(int id)
    {
        var user = _userRepo.Get(id);
        return user.NickName;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var users = _userRepo.All().Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
        {
            Value = c.Id,
            Text = c.NickName,
            ValueStr = c.NickName
        }).ToList();

        return users;
    }

    public IEnumerable<BaseOptionDto> GetNoRoleUsers()
    {
        var userRole = _urRepo.All().Select(c => JsonConvert.DeserializeObject<List<int>>(c.UserIds)).ToList();

        var temp = new List<int>();
        foreach (var roleIds in userRole) temp.AddRange(roleIds);

        var roleUserIds = temp.Distinct();
        var users = _userRepo.All().Where(c => c.Status == Status.Enabled &&
                                               !roleUserIds.Contains(c.Id)).Select(c => new BaseOptionDto
        {
            Value = c.Id,
            Text = c.NickName
        }).ToList();

        return users;
    }
}