using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Services.BaseData.Impl;

public class FactorySiteService : IFactorySiteService
{
    private readonly IRepository<FactorySite> _fsRepo;
    private readonly IRepository<Core.Entities.Specification> _spRepo;
    private readonly IUnitOfWork _uow;

    public FactorySiteService(IRepository<FactorySite> fsRepo,
        IRepository<Core.Entities.Specification> spRepo, IUnitOfWork uow)
    {
        _fsRepo = fsRepo;
        _spRepo = spRepo;
        _uow = uow;
    }

    public IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        total = 0;
        var data = _fsRepo.All().AsNoTracking();
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
                    Id = c.Id,
                    Name = c.Name,
                    Ids = c.SpecificationTypes == null
                        ? new List<int>()
                        : JsonConvert.DeserializeObject<List<int>>(c
                            .SpecificationTypes),
                    State = c.Status == Status.Enabled
                }).ToList();

        return result;
    }

    public bool Add(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_fsRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = "该测试方法已被使用, 请使用其他名称";
            return false;
        }

        var factorySite = new FactorySite
        {
            Name = dto.Name.Trim(),
            SpecificationTypes = JsonConvert.SerializeObject(dto.Ids),
            Status = dto.State ? Status.Enabled : Status.Disabled
        };


        _fsRepo.Add(factorySite);

        return _uow.Save() > 0;
    }

    public bool Update(BaseEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_fsRepo.All()
            .Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = "该测试方法已被使用, 请使用其他名称";
            return false;
        }

        var factorySite = _fsRepo.Get(dto.Id);
        factorySite.Name = dto.Name.Trim();
        factorySite.SpecificationTypes = JsonConvert.SerializeObject(dto.Ids);
        factorySite.Status = dto.State ? Status.Enabled : Status.Disabled;

        // _logService.AddLog($"编辑机台 {dto.Id}, 机台名称为 {dto.Name}");

        _fsRepo.Update(factorySite);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _fsRepo.All().AsNoTracking().OrderByDescending(c => c.ModifiedAtUtc)
            .Where(c => c.Status == Status.Enabled).Select(c => new BaseOptionDto
            {
                Value = c.Id,
                Text = c.Name,
                ValueStr = c.Name
            }).ToList();

        return data;
    }

    public string GetFactorySiteBySpecificationId(int id)
    {
        var specification = _spRepo.Get(id);
        var factorySite = _fsRepo.All()
            .FirstOrDefault(c => c.SpecificationTypes.Contains(specification.SpecificationTypeId.ToString()));
        return factorySite == null ? "" : factorySite.Name;
    }
}