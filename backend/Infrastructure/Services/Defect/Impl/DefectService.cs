using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Defect;
using Core.Entities;
using Core.Enums;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Defect.Impl;

[AutoInject(typeof(IDefectService), InjectType.Scope)]
public class DefectService: IDefectService
{
    private readonly IRepository<Core.Entities.Defect> _dRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<DefectEvents> _deRepo;

    public DefectService(IRepository<Core.Entities.Defect> dRepo, 
        IUnitOfWork uow,
        IRepository<DefectEvents> deRepo)
    {
        _dRepo = dRepo;
        _uow = uow;
        _deRepo = deRepo;
    }
    public IEnumerable<DefectInfoDto> GetDefects(DefectQueryInfoDto dto, out int total)
    {
        var data = _dRepo.All().AsNoTracking();

        if (!string.IsNullOrEmpty(dto.Query))
        {
            data = data.Where(c => c.DefectShortName.Contains(dto.Query) ||
                                   c.Description.Contains(dto.Query) ||
                                   c.DefectType.Name.Contains(dto.Query));
        }

        if (!string.IsNullOrEmpty(dto.TypeId))
        {
            var typeId = int.Parse(dto.TypeId);
            data = data.Where(c => c.DefectTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.EventId))
        {
            var eventId = int.Parse(dto.EventId);
            data = data.Where(c => c.DefectEventsId == eventId);
        }

        total = data.Count();

        var events = _deRepo.All().ToList();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip()).Take(dto.PageSize)
            .Select(c => new DefectInfoDto()
            {
                Id = c.Id, ShortName = c.DefectShortName, TypeId = c.DefectTypeId, TypeName = c.DefectType.Name,
                EventId = c.DefectEventsId,
                Description = c.Description, Code = c.DefectCode, CategoryId = (int)c.DefectCategory,
                CategoryName = c.DefectCategory.toDescription(),
                Score = c.Score, State = c.Status == Status.Enabled
            }).ToList();

        foreach (var item in result)
        {
            if (item.EventId > 0)
            {
                var eventItem = events.FirstOrDefault(c => c.Id == item.EventId);
                item.EventName = eventItem?.Name;
            }
            else
            {
                item.EventName = "";
            }
        }

        return result;
    }

    public bool Edit(DefectInfoDto dto, out string message)
    {
        var modify = dto.Id > 0;
        var defect = new Core.Entities.Defect();
        if (modify)
        {
            if (_dRepo.All().Any(c => c.Id != dto.Id && c.DefectShortName == dto.ShortName))
            {
                message = "该缺陷已存在, 请使用其他名称";
                return false;
            }

            defect = _dRepo.Get(dto.Id);
        }
        else
        {
            if (_dRepo.All().Any(c => c.DefectShortName == dto.ShortName))
            {
                message = "该缺陷已存在, 请使用其他名称";
                return false;
            }
        }

        defect.DefectShortName = dto.ShortName;
        defect.DefectTypeId = dto.TypeId;
        defect.DefectEventsId = dto.EventId;
        defect.DefectCode = dto.Code;
        defect.Description = dto.Description;
        defect.DefectCategory = (DefectCategory)dto.CategoryId;
        defect.Score = dto.Score;
        defect.Status = dto.State ? Status.Enabled : Status.Disabled;
        
        if (modify)
            _dRepo.Update(defect);
        else
            _dRepo.Add(defect);

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;
        message = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";

        return result;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        return _dRepo.All().Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto()
        {
            Value = c.Id, Text = c.DefectShortName
        }).ToList();
    }
}