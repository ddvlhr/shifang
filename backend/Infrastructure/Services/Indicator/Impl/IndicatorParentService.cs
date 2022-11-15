using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Indicator.Impl;

public class IndicatorParentService : IIndicatorParentService
{
    private readonly IRepository<IndicatorParent> _ipRepo;
    private readonly IUnitOfWork _uow;

    public IndicatorParentService(IRepository<IndicatorParent> ipRepo, IUnitOfWork uow)
    {
        _ipRepo = ipRepo;
        _uow = uow;
    }

    public IEnumerable<BaseTableDto> Get(BaseQueryInfoDto dto, out int total)
    {
        var data = _ipRepo.All().AsNoTracking();
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
        if (_ipRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该上级指标名称已被使用, 请使用其他名称";
            return false;
        }

        var indicatorParent = new IndicatorParent
        {
            Name = dto.Name,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _ipRepo.Add(indicatorParent);

        return _uow.Save() > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_ipRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = "该上级指标名称已被使用, 请使用其他名称";
            return false;
        }

        var indicatorParent = _ipRepo.Get(dto.Id);
        indicatorParent.Name = dto.Name;
        indicatorParent.Status =
            dto.State ? Status.Enabled : Status.Disabled;

        _ipRepo.Update(indicatorParent);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetIndicatorParentOptions()
    {
        var options = _ipRepo.All().Where(c => c.Status == Status.Enabled)
            .OrderByDescending(c => c.ModifiedAtUtc).Select(c =>
                new BaseOptionDto
                {
                    Value = c.Id, Text = c.Name
                }).ToList();

        return options;
    }
}