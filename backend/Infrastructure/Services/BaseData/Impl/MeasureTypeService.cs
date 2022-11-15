using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class MeasureTypeService : IMeasureTypeService
{
    private const string _reason = "该测量类型名称已被使用, 请使用其他名称";
    private readonly IRepository<MeasureType> _mtRepo;
    private readonly IUnitOfWork _uow;

    public MeasureTypeService(IRepository<MeasureType> mtRepo, IUnitOfWork uow)
    {
        _mtRepo = mtRepo;
        _uow = uow;
    }

    public IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _mtRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new BaseTableDto
                {
                    Id = c.Id, Name = c.Name,
                    State = c.Status == Status.Enabled
                }).ToList();

        return result;
    }

    public bool Add(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_mtRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = _reason;
            return false;
        }

        var measureType = new MeasureType
        {
            Name = dto.Name,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _mtRepo.Add(measureType);

        return _uow.Save() > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_mtRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = _reason;
            return false;
        }

        var measureType = _mtRepo.Get(dto.Id);
        measureType.Name = dto.Name;
        measureType.Status = dto.State ? Status.Enabled : Status.Disabled;

        _mtRepo.Update(measureType);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _mtRepo.All().AsNoTracking().OrderByDescending(c => c.ModifiedAtUtc)
            .Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
            {
                Value = c.Id, Text = c.Name
            }).ToList();

        return data;
    }

    public int GetTypeIdByTypeName(string typeName)
    {
        var type = _mtRepo.All().FirstOrDefault(c => c.Name.Contains(typeName));
        if (type == null) return 0;
        return type.Id;
    }
}