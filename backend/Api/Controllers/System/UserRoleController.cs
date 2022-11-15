using Core.Dtos.UserRole;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.System;

[Authorize]
public class UserRoleController : BaseController
{
    private readonly IUserRoleService _urService;

    public UserRoleController(IUserRoleService urService)
    {
        _urService = urService;
    }

    [HttpGet]
    [Route("userRole/{id}")]
    public IActionResult Get(int id)
    {
        return Success(_urService.Get(id));
    }

    [HttpPost]
    [Route("userRole")]
    public IActionResult Add([FromBody] UserRoleEditDto dto)
    {
        return _urService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("userRole")]
    public IActionResult Update([FromBody] UserRoleEditDto dto)
    {
        return _urService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }
}