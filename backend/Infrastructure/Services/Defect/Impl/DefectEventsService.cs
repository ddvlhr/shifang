using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Defect.Impl;

[AutoInject(typeof(IDefectEventsService), InjectType.Scope)]
public class DefectEventsService: IDefectEventsService
{
    private readonly IRepository<DefectEvents> _deRepo;
    private readonly IUnitOfWork _uow;

    public DefectEventsService(IRepository<DefectEvents> deRepo, IUnitOfWork uow)
    {
        _deRepo = deRepo;
        _uow = uow;
    }
    public IEnumerable<BaseTableDto> GetDefectEvents(BaseQueryInfoDto dto, out int total)
    {
        var data = _deRepo.All().AsNoTracking();
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
            }).ToList();

        return result;
    }

    public bool Edit(BaseEditDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var defectEvents = new DefectEvents();
        if (modify)
        {
            if (_deRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
            {
                message = "该缺陷类别小项已存在, 请使用其他名称";
                return false;
            }

            defectEvents = _deRepo.Get(dto.Id);
        }
        else
        {
            if (_deRepo.All().Any(c => c.Name == dto.Name))
            {
                message = "该缺陷类别小项已存在, 请使用其他名称";
                return false;
            }
        }

        defectEvents.Name = dto.Name;
        defectEvents.Status = dto.State ? Status.Enabled : Status.Disabled;
        
        if (modify)
            _deRepo.Update(defectEvents);
        else
            _deRepo.Add(defectEvents);

        var result = modify ? _uow.Save() > 0 : _uow.Save() >= 0;
        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";

        return result;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        return _deRepo.All().Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto()
        {
            Value = c.Id, Text = c.Name
        }).ToList();
    }
}