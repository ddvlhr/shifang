using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Function;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.System.Impl;

public class FunctionService : IFunctionService
{
    private readonly IRepository<Function> _functionRepo;
    private readonly IRepository<Menu> _menuRepo;
    private readonly IUnitOfWork _uow;

    public FunctionService(IRepository<Function> functionRepo, IRepository<Menu> menuRepo, IUnitOfWork uow)
    {
        _functionRepo = functionRepo;
        _menuRepo = menuRepo;
        _uow = uow;
    }

    public bool Add(EditFunctionDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_functionRepo.All().Any(c => c.Name == dto.Name && c.MenuId == dto.MenuId))
        {
            failReason = "该功能名称已存在, 请使用其他名称";
            return false;
        }

        if (_functionRepo.All()
            .Any(c => c.FunctionName == dto.FunctionName && c.MenuId == dto.MenuId))
        {
            failReason = "该方法名称已存在, 请使用其他名称";
            return false;
        }

        var function = new Function
        {
            Id = dto.Id,
            Name = dto.Name,
            FunctionName = dto.FunctionName,
            Type = dto.Type,
            MenuId = dto.MenuId,
            Position = (FunctionPosition)dto.Position,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _functionRepo.Add(function);

        return _uow.Save() > 0;
    }

    public IEnumerable<FunctionTableDto> GetMenuFunctions(int id)
    {
        var functions = _functionRepo.All().Where(c => c.MenuId == id)
            .Select(c => new FunctionTableDto
            {
                Id = c.Id, Name = c.Name, FunctionName = c.FunctionName,
                Type = c.Type, State = c.Status == Status.Enabled, Position = (int)c.Position
            }).ToList();

        return functions;
    }

    public EditFunctionDto GetFunction(int id)
    {
        var function = _functionRepo.Get(id);
        var dto = new EditFunctionDto
        {
            Id = function.Id,
            Name = function.Name,
            FunctionName = function.FunctionName,
            Type = function.Type,
            Position = (int)function.Position,
            State = function.Status == Status.Enabled
        };

        return dto;
    }

    public bool Update(EditFunctionDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_functionRepo.All()
            .Any(c => c.Id != dto.Id && c.Name == dto.Name && c.MenuId == dto.MenuId))
        {
            failReason = "该菜单下已存在此功能";
            return false;
        }

        if (_functionRepo.All()
            .Any(c => c.Id != dto.Id && c.FunctionName == dto.FunctionName && c.MenuId == dto.MenuId))
        {
            failReason = "该菜单下已存在此方法";
            return false;
        }

        var function = _functionRepo.Get(dto.Id);
        function.Name = dto.Name;
        function.FunctionName = dto.FunctionName;
        function.Type = dto.Type;
        function.Position = (FunctionPosition)dto.Position;
        function.Status = dto.State ? Status.Enabled : Status.Disabled;

        _functionRepo.Update(function);

        return _uow.Save() >= 0;
    }

    public bool Delete(int id, out string failReason)
    {
        failReason = string.Empty;
        var function = _functionRepo.Get(id);
        if (function == null)
        {
            failReason = "没有找到对应的功能";
            return false;
        }

        _functionRepo.Delete(function);

        return _uow.Save() > 0;
    }

    public IEnumerable<BaseTreeDto> GetMenuFunctions()
    {
        var data = _menuRepo.All().AsNoTracking()
            .OrderByDescending(c => c.Level).Where(c => c.Status == Status.Enabled && c.Level != 0).Select(c =>
                new BaseTreeDto
                {
                    Id = c.Id, Label = c.Name, Children = _functionRepo.All()
                        .Where(x => x.MenuId == c.Id && x.Status == Status.Enabled).Select(x => new BaseTreeDto.Child
                        {
                            Id = x.Id + 200, Label = x.Name, ParentId = x.MenuId
                        }).ToList()
                }).ToList();

        return data;
    }
}