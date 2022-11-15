using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Services.Specification.Impl;

public class SpecificationTypeRuleService : ISpecificationTypeRuleService
{
    private readonly IRepository<SpecificationType> _stRepo;
    private readonly IRepository<SpecificationTypeRule> _strRepo;
    private readonly IUnitOfWork _uow;

    public SpecificationTypeRuleService(IRepository<SpecificationTypeRule> strRepo,
        IRepository<SpecificationType> stRepo, IUnitOfWork uow)
    {
        _strRepo = strRepo;
        _stRepo = stRepo;
        _uow = uow;
    }

    public IEnumerable<SpecificationTypeRuleTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _stRepo.All().AsNoTracking().Where(c => c.Status == Status.Enabled);
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        total = data.Count();

        var result = data.Skip(dto.Skip()).Take(dto.PageSize).Select(c => new SpecificationTypeRuleTableDto
        {
            Id = c.Id, Name = c.Name, Count = _strRepo.All().FirstOrDefault(x => x.SpecificationTypeId == c.Id) ==
                                              null
                ? 0
                : JsonConvert.DeserializeObject<List<Rule>>(_strRepo.All().FirstOrDefault(x => x
                    .SpecificationTypeId == c.Id).Rules).ToList().Count()
        }).ToList();

        return result;
    }

    public SpecificationTypeRuleEditDto Get(int id)
    {
        var dto = new SpecificationTypeRuleEditDto();
        var type = _stRepo.Get(id);
        var data = _strRepo.All()
            .FirstOrDefault(c => c.SpecificationTypeId == id);
        if (data != null)
        {
            dto.Id = data.Id;
            dto.TypeId = data.SpecificationTypeId;
            dto.TypeName = type.Name;
            dto.Rules = JsonConvert.DeserializeObject<List<Rule>>(data.Rules);
        }
        else
        {
            dto.TypeId = id;
            dto.TypeName = type.Name;
            dto.Rules = new List<Rule>();
        }

        return dto;
    }

    public bool Edit(SpecificationTypeRuleEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        var isEdit = dto.Id > 0;
        var entity = new SpecificationTypeRule();
        if (isEdit) entity = _strRepo.Get(dto.Id);

        entity.SpecificationTypeId = dto.TypeId;
        entity.Rules = JsonConvert.SerializeObject(dto.Rules);

        if (isEdit)
            _strRepo.Update(entity);
        else
            _strRepo.Add(entity);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;

        return result;
    }
}