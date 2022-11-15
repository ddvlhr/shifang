using Core.Dtos;
using Core.Dtos.Role;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.System;

[Authorize]
public class RoleController : BaseController
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [Route("role")]
    public IActionResult Get([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _roleService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("role")]
    public IActionResult Post([FromBody] RoleEditDto dto)
    {
        return _roleService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpPut]
    [Route("role")]
    public IActionResult Put([FromBody] RoleEditDto dto)
    {
        return _roleService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet, Route("system/role/options")]
    public IActionResult GetOptions()
    {
        return Success(_roleService.GetOptions());
    }
}