using Core.Dtos;
using Core.Dtos.Menu;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.System;

[Authorize]
public class MenuController : BaseController
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpPost]
    [Route("menu")]
    public IActionResult Post([FromBody] EditMenuDto dto)
    {
        return _menuService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("menu")]
    public IActionResult Put([FromBody] EditMenuDto dto)
    {
        return _menuService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("menu")]
    public IActionResult Get([FromQuery] BaseQueryInfoDto dto)
    {
        var menus = _menuService.GetMenus(dto, out var total);
        return Success(new { total, result = menus });
    }

    [HttpDelete]
    [Route("menu/{id}")]
    public IActionResult Delete(int id)
    {
        return _menuService.Delete(id, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("menu/aside/{id}")]
    public IActionResult GetAside(int id)
    {
        var menus = _menuService.GetAsideMenuList(id);
        return Success(menus);
    }

    [HttpGet]
    [Route("menu/root")]
    public IActionResult GetRootMenus()
    {
        var roots = _menuService.GetRootMenus();

        return Success(roots);
    }
}