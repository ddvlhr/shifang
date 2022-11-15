using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

[AutoInject(typeof(IDepartmentService), InjectType.Scope)]
public class DepartmentService: IDepartmentService
{
    private readonly IRepository<Department> _dRepo;
    private readonly IUnitOfWork _uow;

    public DepartmentService(IRepository<Department> dRepo,
        IUnitOfWork uow)
    {
        _dRepo = dRepo;
        _uow = uow;
    }
    public IEnumerable<BaseTableDto> GetDepartments(BaseQueryInfoDto dto, out int total)
    {
        var data = _dRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query))
        {
            data = data.Where(c => c.Name.Contains(dto.Query));
        }

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();

        var result = data.OrderByDescending(c => c.ModifiedAtUtc)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c => new BaseTableDto()
            {
                Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled
            });

        return result;
    }

    public bool Edit(BaseEditDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var department = new Department();
        if (modify)
        {
            if (_dRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
            {
                message = "该部门已存在, 请使用其他名称";
                return false;
            }

            department = _dRepo.Get(dto.Id);
        }
        else
        {
            if (_dRepo.All().Any(c => c.Name == dto.Name))
            {
                message = "该部门已存在, 请使用其他名称";
                return false;
            }
        }

        department.Name = dto.Name;
        department.Status = dto.State ? Status.Enabled : Status.Disabled;
        
        if (modify)
            _dRepo.Update(department);
        else
            _dRepo.Add(department);

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;
        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";
        return result;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        return _dRepo.All().OrderByDescending(c => c.ModifiedAtUtc).Where(c => c.Status == Status.Enabled)
            .Select(c => new BaseOptionDto()
            {
                Value = c.Id, Text = c.Name
            }).ToList();
    }
}