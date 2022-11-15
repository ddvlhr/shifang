using System.Collections.Generic;
using Core.Dtos.Permission;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
public class PermissionController : BaseController
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet, Route("permission/tree/{role}")]
    public IActionResult GetPermissionTree(int role)
    {
        return Success(_permissionService.GetPermissionTree(role));
    }

    [HttpPost, Route("permission")]
    public IActionResult Edit([FromBody] EditPermissionDto dto)
    {
        return _permissionService.Edit(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpDelete, Route("permission")]
    public IActionResult Remove([FromBody] List<int> ids)
    {
        return _permissionService.Remove(ids, out var message) ? Success(msg: message) : Error(message);
    }
    
    [HttpGet, Route("permission/options/{root}")]
    public IActionResult GetOptions(bool root)
    {
        return Success(_permissionService.GetOptions(root));
    }
}