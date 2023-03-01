using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Indicator;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Core.Dtos.Specification;

namespace Infrastructure.Services.Indicator.Impl;

public class IndicatorService : IIndicatorService
{
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IUnitOfWork _uow;

    public IndicatorService(IRepository<Core.Entities.Indicator> iRepo, IUnitOfWork uow)
    {
        _iRepo = iRepo;
        _uow = uow;
    }

    public IEnumerable<IndicatorTableDto> Get(IndicatorQueryInfoDto dto, out int total)
    {
        var data = _iRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c =>
                c.Name.Contains(dto.Query) ||
                c.IndicatorParent.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.Parent))
        {
            var parentId = int.Parse(dto.Parent);
            data = data.Where(c => c.IndicatorParentId == parentId);
        }

        if (!string.IsNullOrEmpty(dto.Project))
        {
            var project = int.Parse(dto.Project);
            data = data.Where(c => c.IndicatorProject == (IndicatorProject)project);
        }

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc).Include(c => c.IndicatorParent)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new IndicatorTableDto
                {
                    Id = c.Id, Name = c.Name,
                    Parent = c.IndicatorParentId,
                    Unit = c.Unit,
                    Standard = c.Standard,
                    Score = c.Score,
                    ParentName = c.IndicatorParent.Name,
                    Project = (int)c.IndicatorProject,
                    State = c.Status == Status.Enabled
                }).ToList();
        return result;
    }

    public bool Add(IndicatorEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_iRepo.All().Any(c => c.Name == dto.Name && c.IndicatorParentId == dto.Parent))
        {
            failReason = "该指标名称已存在, 请使用其他名称";
            return false;
        }

        var indicator = new Core.Entities.Indicator
        {
            Name = dto.Name,
            Unit = dto.Unit,
            Standard = dto.Standard,
            Score = dto.Score,
            IndicatorParentId = dto.Parent,
            IndicatorProject = (IndicatorProject)dto.Project,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _iRepo.Add(indicator);

        return _uow.Save() > 0;
    }

    public bool Update(IndicatorEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_iRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name && c.IndicatorParentId == dto.Parent))
        {
            failReason = "该指标名称已存在, 请使用其他名称";
            return false;
        }

        var indicator = _iRepo.Get(dto.Id);
        indicator.Name = dto.Name;
        indicator.Unit = dto.Unit;
        indicator.Standard = dto.Standard;
        indicator.Score = dto.Score;
        indicator.IndicatorParentId = dto.Parent;
        indicator.IndicatorProject = (IndicatorProject)dto.Project;
        indicator.Status = dto.State ? Status.Enabled : Status.Disabled;

        _iRepo.Update(indicator);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetIndicatorOptions(IndicatorProject project)
    {
        var data = new List<BaseOptionDto>();
        if (project == IndicatorProject.Measure)
            data = _iRepo.All().Include(c => c.IndicatorParent).Where(c =>
                    c.Status == Status.Enabled && c.IndicatorProject == IndicatorProject.Measure)
                .OrderBy(c => c.IndicatorParentId).Select(c =>
                    new BaseOptionDto
                    {
                        Value = c.Id,
                        Text = $"{c.Name} ({c.IndicatorParent.Name})"
                    }).ToList();
        else if (project == IndicatorProject.Appearance)
            data = _iRepo.All().Include(c => c.IndicatorParent).Where(c =>
                    c.Status == Status.Enabled && c.IndicatorProject == IndicatorProject.Appearance)
                .OrderBy(c => c.IndicatorParentId).Select(c =>
                    new BaseOptionDto
                    {
                        Value = c.Id,
                        Text = $"{c.Name} ({c.IndicatorParent.Name})"
                    }).ToList();
        else if (project == IndicatorProject.Craft)
            data = _iRepo.All().Include(c => c.IndicatorParent).Where(c =>
                    c.Status == Status.Enabled && c.IndicatorProject == IndicatorProject.Craft)
                .OrderBy(c => c.IndicatorParentId).Select(c =>
                    new BaseOptionDto
                    {
                        Value = c.Id,
                        Text = $"{c.Name} ({c.IndicatorParent.Name})"
                    }).ToList();
        else
            data = _iRepo.All().Include(c => c.IndicatorParent).Where(c => c.Status == Status.Enabled)
                .OrderBy(c => c.IndicatorParentId).Select(c =>
                    new BaseOptionDto
                    {
                        Value = c.Id,
                        Text = $"{c.Name} ({c.IndicatorParent.Name})"
                    }).ToList();


        return data;
    }

    public IEnumerable<Rule> GetRuleList(List<int> ids)
    {
        var data = _iRepo.All().Where(c => ids.Contains(c.Id)).Select(c =>
            new Rule
            {
                Id = c.Id, Name = c.Name
            }).ToList();

        return data;
    }
}