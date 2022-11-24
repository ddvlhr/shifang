using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Permission;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Infrastructure.Extensions;

namespace Infrastructure.Services.System.Impl;

[AutoInject(typeof(IPermissionService), InjectType.Scope)]
public class PermissionService : IPermissionService
{
    private readonly IRepository<Permission> _permissionRepository;
    private readonly IUnitOfWork _uow;

    public PermissionService(IRepository<Permission> permissionRepository,
        IUnitOfWork uow)
    {
        _permissionRepository = permissionRepository;
        _uow = uow;
    }

    public IEnumerable<PermissionTreeDto> GetPermissionTree(int role)
    {
        var tree = new List<PermissionTreeDto>();
        if (role == 0)
        {
            var permissions = _permissionRepository.All().ToList();
            var root = permissions.Where(c => c.Level == 0).OrderBy(c => c.Order).ToList();
            foreach (var item in root)
            {
                var rootItem = new PermissionTreeDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Icon = item.Icon,
                    PermissionType = (int)item.PermissionType,
                    Order = item.Order
                };
                var children = permissions.Where(c => c.Level == item.Id).OrderBy(c => c.Order).ToList();
                var menuItems = new List<PermissionTreeDto.MenuItem>();
                if (children.Count > 0)
                {
                    foreach (var child in children)
                    {
                        var menuItem = new PermissionTreeDto.MenuItem()
                        {
                            Id = child.Id,
                            Level = child.Level,
                            Name = child.Name,
                            PermissionType = (int)child.PermissionType,
                            Path = child.Path,
                            Component = child.Component,
                            Order = child.Order
                        };
                        var functions = permissions
                            .Where(c => c.Level == child.Id && c.PermissionType == PermissionType.Button)
                            .OrderBy(c => c.Order).ToList();
                        var functionItems = new List<PermissionTreeDto.FunctionItem>();
                        if (functions.Count > 0)
                        {
                            foreach (var function in functions)
                            {
                                var functionItem = new PermissionTreeDto.FunctionItem()
                                {
                                    Id = function.Id,
                                    Level = function.Level,
                                    Name = function.Name,
                                    FunctionName = function.FunctionName,
                                    PermissionType = (int)function.PermissionType,
                                    ButtonType = (int)function.ButtonType,
                                    ButtonTypeName = function.ButtonType.toDescription(),
                                    ButtonPosition = (int)function.ButtonPosition,
                                    Order = function.Order
                                };
                                functionItems.Add(functionItem);
                            }
                        }

                        menuItem.Children = functionItems;
                        menuItems.Add(menuItem);
                    }
                }

                rootItem.Children = menuItems;
                tree.Add(rootItem);
            }
        }

        return tree;
    }

    public bool Edit(EditPermissionDto dto, out string failReason)
    {
        failReason = "";
        var modify = dto.Id > 0;

        var permissionType = (PermissionType)dto.PermissionType;
        var permission = new Permission();

        if (modify)
        {
            permission = _permissionRepository.Get(dto.Id);
        }

        permission.Name = dto.Name.Trim();
        permission.PermissionType = permissionType;
        permission.Level = dto.Level;
        permission.Order = dto.Order;

        if (permissionType == PermissionType.Menu)
        {
            if (dto.Level > 0)
            {
                permission.Path = dto.Path.Trim();
                permission.Component = dto.Component.Trim();
            }
            else
            {
                permission.Icon = dto.Icon.Trim();
            }
        }
        else
        {
            permission.FunctionName = dto.FunctionName.Trim();
            permission.ButtonType = (ButtonType)dto.ButtonType;
            permission.ButtonPosition = (ButtonPosition)dto.ButtonPosition;
        }

        if (modify)
        {
            _permissionRepository.Update(permission);
        }
        else
        {
            _permissionRepository.Add(permission);
        }

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;
        return result;
    }

    public bool Remove(List<int> ids, out string message)
    {
        var permissions = _permissionRepository.All().Where(c => ids.Contains(c.Id)).ToList();
        if (permissions.Count == 0)
        {
            message = "没有找到需要删除的数据";
            return false;
        }

        _permissionRepository.DeleteRange(permissions);

        var result = _uow.Save() > 0;
        message = result ? "删除成功" : "删除失败";
        return result;
    }

    public IEnumerable<BaseOptionDto> GetOptions(bool root)
    {
        List<BaseOptionDto> result;
        if (root)
        {
            result = _permissionRepository.All().Where(c => c.Level == 0).OrderBy(c => c.Order).Select(c =>
                new BaseOptionDto()
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList();
        }
        else
        {
            result = _permissionRepository.All().Where(c => c.Level != 0 && c.PermissionType == PermissionType.Menu)
                .OrderByDescending(c=>c.ModifiedAtUtc).Select(c => new BaseOptionDto()
                {
                    Value = c.Id,
                    Text = c.Name
                }).ToList();
        }

        return result;
    }
}