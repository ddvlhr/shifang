using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Menu;

namespace Infrastructure.Services.System;

public interface IMenuService
{
    bool Add(EditMenuDto dto, out string failReason);
    bool Update(EditMenuDto dto, out string failReason);
    bool Delete(int id, out string failReason);
    IEnumerable<AsideMenuDto> GetAsideMenuList(int id);
    IEnumerable<MenuTableDto> GetMenus(BaseQueryInfoDto dto, out int total);
    IEnumerable<MenuOptionDto> GetRootMenus();
}