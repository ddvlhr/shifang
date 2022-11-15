using Core.Dtos;
using Core.Dtos.User;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.System;

[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("user")]
    public IActionResult Post([FromBody] EditUserDto dto)
    {
        return _userService.AddUser(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("user")]
    public IActionResult Put([FromBody] EditUserDto dto)
    {
        return _userService.EditUser(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("user")]
    public IActionResult Get([FromQuery] BaseQueryInfoDto dto)
    {
        var users = _userService.GetUserTable(dto, out var total);
        return Success(new { list = users, total });
    }

    [HttpPost]
    [Route("user/modifyPassword")]
    public IActionResult ModifyPassword([FromBody] ModifyPasswordDto dto)
    {
        return _userService.ModifyPassword(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("user/resetPassword")]
    public IActionResult ResetPassword(int id)
    {
        return _userService.ResetPassword(id, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet, Route("system/user/options")]
    public IActionResult GetOptions()
    {
        return Success(_userService.GetOptions());
    }
}