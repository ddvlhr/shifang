using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;

namespace Infrastructure.Services.BaseData.Impl;

public class TeamService: ITeamService
{
    private readonly IRepository<Team> _teamRepository;
    private readonly IUnitOfWork _uow;

    public TeamService(IRepository<Team> teamRepository,
        IUnitOfWork uow)
    {
        _teamRepository = teamRepository;
        _uow = uow;
    }
    public IEnumerable<BaseTableDto> GetTeams(BaseQueryInfoDto dto, out int total)
    {
        var data = _teamRepository.All();

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

        var result = data.OrderByDescending(c => c.ModifiedAtUtc).Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
            new BaseTableDto()
            {
                Id = c.Id, Name = c.Name, State = c.Status == Status.Enabled
            }).ToList();

        return result;
    }

    public bool Edit(BaseEditDto dto, out string failReason)
    {
        var modify = dto.Id > 0;
        var team = new Team();
        if (modify)
        {
            if (_teamRepository.All().Any(c => c.Name == dto.Name && c.Id != dto.Id))
            {
                failReason = "该班组已存在, 请使用其他名称";
                return false;
            }

            team = _teamRepository.Get(dto.Id);
        }
        else
        {
            if (_teamRepository.All().Any(c => c.Name == dto.Name))
            {
                failReason = "该班组已存在, 请使用其他名称";
                return false;
            }
        }

        team.Name = dto.Name;
        team.Status = dto.State ? Status.Enabled : Status.Disabled;

        if (modify)
        {
            _teamRepository.Update(team);
        }
        else
        {
            _teamRepository.Add(team);
        }

        var result = modify ? _uow.Save() >= 0 : _uow.Save() > 0;

        failReason = result ? modify ? "修改成功" : "添加成功" : modify ? "修改失败" : "添加失败";

        return result;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var options = _teamRepository.All().Where(c => c.Status == Status.Enabled)
            .OrderByDescending(c => c.ModifiedAtUtc).Select(c => new BaseOptionDto()
            {
                Value = c.Id, Text = c.Name
            }).ToList();
        return options;
    }
}