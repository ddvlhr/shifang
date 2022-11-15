using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Services.Specification.Impl;

public class CraftIndicatorRuleService : ICraftIndicatorRuleService
{
    private readonly IRepository<CraftIndicatorRule> _cRepo;
    private readonly IRepository<Model> _mRepo;
    private readonly IUnitOfWork _uow;

    public CraftIndicatorRuleService(IRepository<CraftIndicatorRule> cRepo, IRepository<Model> mRepo, IUnitOfWork uow)
    {
        _cRepo = cRepo;
        _mRepo = mRepo;
        _uow = uow;
    }

    public IEnumerable<CraftIndicatorTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _mRepo.All().AsNoTracking().Where(c => c.Status == Status.Enabled);
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        total = data.Count();

        var result = data.Skip(dto.Skip()).Take(dto.PageSize).Select(c => new CraftIndicatorTableDto
        {
            Id = c.Id,
            Name = c.Name,
            Count = _cRepo.All().FirstOrDefault(x => x.ModelId == c.Id) ==
                    null
                ? 0
                : JsonConvert.DeserializeObject<List<Rule>>(_cRepo.All().FirstOrDefault(x => x
                    .ModelId == c.Id).Rules).ToList().Count()
        }).ToList();

        return result;
    }

    public bool Edit(CraftIndicatorEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        var isEdit = dto.Id > 0;
        var entity = new CraftIndicatorRule();
        if (isEdit) entity = _cRepo.Get(dto.Id);

        entity.ModelId = dto.ModelId;
        entity.Rules = JsonConvert.SerializeObject(dto.Rules);

        if (isEdit)
            _cRepo.Update(entity);
        else
            _cRepo.Add(entity);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;

        return result;
    }

    public CraftIndicatorEditDto Get(int id)
    {
        var dto = new CraftIndicatorEditDto();
        var type = _mRepo.Get(id);
        var data = _cRepo.All()
            .FirstOrDefault(c => c.ModelId == id);
        if (data != null)
        {
            dto.Id = data.Id;
            dto.ModelId = data.ModelId;
            dto.ModelName = type.Name;
            dto.Rules = JsonConvert.DeserializeObject<List<Rule>>(data.Rules);
        }
        else
        {
            dto.ModelId = id;
            dto.ModelName = type.Name;
            dto.Rules = new List<Rule>();
        }

        return dto;
    }
}