using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Function;
using Core.Dtos.Menu;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Services.System.Impl;

public class MenuService : IMenuService
{
    private readonly IRepository<Function> _fRepo;
    private readonly IFunctionService _functionService;
    private readonly IRepository<Menu> _menuRepo;
    private readonly IRepository<Role> _rRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<UserRole> _urRepo;

    public MenuService(IRepository<Menu> menuRepo, IRepository<Role> rRepo, IRepository<Function> fRepo,
        IRepository<UserRole> urRepo, IUnitOfWork uow, IFunctionService functionService)
    {
        _menuRepo = menuRepo;
        _rRepo = rRepo;
        _fRepo = fRepo;
        _urRepo = urRepo;
        _uow = uow;
        _functionService = functionService;
    }

    public bool Add(EditMenuDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_menuRepo.All().Any(c => c.Name == dto.Name && c.Level == dto.Level))
        {
            failReason = "该菜单已存在, 请使用其他菜单名称";
            return false;
        }

        var menu = new Menu
        {
            Name = dto.Name,
            Url = dto.Path,
            Icon = dto.Icon,
            Level = dto.Level,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _menuRepo.Add(menu);

        return _uow.Save() > 0;
    }

    public bool Update(EditMenuDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_menuRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name && c.Level == dto.Level))
        {
            failReason = "该菜单名称已存在, 请使用其他菜单名称";
            return false;
        }

        var menu = _menuRepo.Get(dto.Id);
        menu.Name = dto.Name;
        menu.Icon = dto.Icon;
        menu.Url = dto.Path;
        menu.Level = dto.Level;
        menu.Status = dto.State ? Status.Enabled : Status.Disabled;

        _menuRepo.Update(menu);

        return _uow.Save() >= 0;
    }

    public bool Delete(int id, out string failReason)
    {
        failReason = string.Empty;
        var menu = _menuRepo.Get(id);
        if (menu == null)
        {
            failReason = "没有找到该菜单对应的数据.";
            return false;
        }

        _menuRepo.Delete(menu);

        return _uow.Save() > 0;
    }

    public IEnumerable<MenuTableDto> GetMenus(BaseQueryInfoDto dto, out int total)
    {
        var skip = (dto.PageNum - 1) * dto.PageSize;
        var menus = _menuRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) menus = menus.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            menus = menus.Where(c => c.Status == (Status)state);
        }

        total = menus.Count();
        var result = menus.Where(c => c.Level == 0).Select(c =>
            new MenuTableDto
            {
                Id = c.Id, Name = c.Name, Icon = c.Icon, Level = c.Level,
                State = c.Status == Status.Enabled, Children = menus
                    .Where(x => x.Level == c.Id).Select(x =>
                        new MenuTableDto.Child
                        {
                            Id = x.Id, Name = x.Name, Path = x.Url, Level = x.Level,
                            State = x.Status == Status.Enabled
                        }).ToList()
            }).Skip(skip).Take(dto.PageSize).ToList();

        return result;
    }

    public IEnumerable<MenuOptionDto> GetRootMenus()
    {
        var result = new List<MenuOptionDto>
        {
            new()
            {
                Value = 0,
                Text = "根目录"
            }
        };
        var roots = _menuRepo.All()
            .Where(c => c.Status == Status.Enabled && c.Level == 0).Select(
                c => new MenuOptionDto
                {
                    Text = c.Name, Value = c.Id
                }).ToList();

        return result.Concat(roots);
    }

    public IEnumerable<AsideMenuDto> GetAsideMenuList(int id)
    {
        var result = new List<AsideMenuDto>();
        var userRoles = _urRepo.All().ToList();
        var roleIds = new List<int>();
        foreach (var userRole in userRoles)
        {
            var userIds = JsonConvert.DeserializeObject<List<int>>(userRole.UserIds);
            if (userIds == null)
            {
                return null;
            }

            var flag = userIds.Any(userId => userId == id);

            if (flag)
            {
                roleIds.Add(userRole.RoleId);
            }
        }
        var roleMenu = _rRepo.All().Where(c => roleIds.Contains(c.Id)).Select(c => c.RoleMenu).ToList();
        // var role = _rRepo.Get(id);
        if (id == 0)
        {
            var menus = _menuRepo.All().Where(c => c.Status == Status.Enabled).ToList();
            var roots = menus.Where(c => c.Level == 0).OrderByDescending(c => c.CreatedAtUtc).ToList();
            foreach (var root in roots)
            {
                var item = new AsideMenuDto
                {
                    Id = root.Id,
                    Name = root.Name,
                    Icon = root.Icon
                };
                var children = menus.Where(c => c.Level == root.Id)
                    .OrderBy(c => c.CreatedAtUtc).Select(c =>
                        new AsideMenuDto.Child
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Path = c.Url,
                            Functions = _functionService.GetMenuFunctions(c.Id)
                        }).ToList();
                item.Children = children;
                result.Add(item);
            }
        }
        else
        {
            var roleMenus = new List<int>();
            foreach (var menu in roleMenu) roleMenus.AddRange(JsonConvert.DeserializeObject<List<int>>(menu));

            roleMenus = roleMenus.Distinct().ToList();
            var functionIds = roleMenus.Where(c => c > 200).Select(c => c - 200).ToList();
            var functions = _fRepo.All().Where(c => functionIds.Contains(c.Id)).Select(c => new FunctionTableDto
            {
                Id = c.Id,
                FunctionName = c.FunctionName,
                MenuId = c.MenuId,
                Name = c.Name,
                Position = (int)c.Position,
                State = c.Status == Status.Enabled,
                Type = c.Type
            }).ToList();
            var menuIds = roleMenus.Where(c => c < 200).ToList();
            var menus = _menuRepo.All().Where(c => c.Status == Status.Enabled && menuIds.Contains(c.Id)).ToList();
            var rootIds = menus.Select(c => c.Level).Distinct().ToList();
            var roots = _menuRepo.All().Where(c => rootIds.Contains(c.Id)).OrderByDescending(c => c.CreatedAtUtc)
                .ToList();
            foreach (var root in roots)
            {
                var item = new AsideMenuDto
                {
                    Id = root.Id,
                    Name = root.Name,
                    Icon = root.Icon
                };
                var children = menus.Where(c => c.Level == root.Id)
                    .OrderBy(c => c.CreatedAtUtc).Select(c =>
                        new AsideMenuDto.Child
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Path = c.Url,
                            Functions = functions.Where(x => x.MenuId == c.Id).ToList()
                        }).ToList();
                item.Children = children;
                result.Add(item);
            }
        }


        return result;
    }
}