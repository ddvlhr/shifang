using Core.Dtos.Authorize;
using Infrastructure.Services.Authorize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     登录验证
/// </summary>
public class AuthenticationController : BaseController
{
    private readonly IAuthenticateService _authenticationService;

    public AuthenticationController(IAuthenticateService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    ///     登录
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <returns>返回token</returns>
    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] LoginUserDto user)
    {
        if (_authenticationService.IsAuthenticated(user, out var token, out var userData, out var failReason))
            return Success(new { token, userInfo = userData });

        return Error(failReason);
    }

    /// <summary>
    ///     测试用户认证
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    [Route("test")]
    public IActionResult Test()
    {
        return Success();
    }
}