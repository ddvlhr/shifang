using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Core.Dtos.Authorize;
using Core.Dtos.User;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Services.System;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Authorize.Impl;

public class AuthenticateService : IAuthenticateService
{
    private readonly IRepository<Role> _rRepo;
    private readonly Token _token;
    private readonly IUserRoleService _urService;
    private readonly IUserService _userService;

    public AuthenticateService(IUserService userService, IUserRoleService urService,
        IOptionsSnapshot<Token> token, IRepository<Role> rRepo)
    {
        _userService = userService;
        _urService = urService;
        _rRepo = rRepo;
        _token = token.Value;
    }

    public bool IsAuthenticated(LoginUserDto dto, out string token, out UserInfoDto userData, out string failReason)
    {
        token = string.Empty;
        userData = new UserInfoDto();

        if (!_userService.IsValid(dto, out var userInfo, out failReason))
            return false;

        userData = userInfo;
        var userRole = _urService.GetRoleIdByUserId(userInfo.Id);
        var roleIds = _urService.GetRoleIdsByUserId(userInfo.Id);
        // 获取用户所有角色
        var roles = _rRepo.All().Where(c => roleIds.Contains(c.Id)).ToList();
        var claims = new[]
        {
            new Claim(ClaimTypes.UserData, userInfo.Id.ToString()),
            new Claim(ClaimTypes.Name, userInfo.NickName),
            new Claim(ClaimTypes.Role, userRole.Role.Name),
            new Claim("roleId", userInfo.RoleId.ToString()),
            // 当有个角色有查看其它用户数据权限时即可查看其它用户数据
            new Claim("canSeeOtherData", roles.Any(c => c.CanSeeOtherData) ? "1" : "0"),
            new Claim("equipmentType", ((int)roles.Min(c=>c.EquipmentType)).ToString())
        };

        var key =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.Secret));
        var credentials =
            new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtToken = new JwtSecurityToken(_token.Issuer, _token.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(_token.AccessExpiration),
            signingCredentials: credentials);

        token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return true;
    }
}