using System.Collections.Generic;
using System.Linq;
using Core.Dtos.Material;
using Core.Entities;
using Infrastructure.DataBase;

namespace Infrastructure.Services.Material.Impl;

public class MaterialTemplateService : IMaterialTemplateService
{
    private readonly IRepository<MaterialTemplate> _mtRepo;
    private readonly IRepository<SpecificationType> _stRepo;
    private readonly IUnitOfWork _uow;

    public MaterialTemplateService(IRepository<MaterialTemplate> mtRepo, IRepository<SpecificationType> stRepo,
        IUnitOfWork uow)
    {
        _mtRepo = mtRepo;
        _stRepo = stRepo;
        _uow = uow;
    }

    public IEnumerable<MaterialTemplateTableDto> GetTable(MaterialTemplateQueryInfoDto dto, out int total)
    {
        var data = _stRepo.All().Where(c => c.Status == Status.Enabled);
        if (!string.IsNullOrEmpty(dto.TypeId))
        {
            var typeId = int.Parse(dto.TypeId);
            data = data.Where(c => c.Id == typeId);
        }

        total = data.Count();

        var result = data.Skip(dto.Skip()).Take(dto.PageSize).Select(c => new MaterialTemplateTableDto
        {
            Id = c.Id, Name = c.Name, Description = _mtRepo.All().FirstOrDefault(x => x.SpecificationTypeId == c
                .Id) == null
                ? ""
                : _mtRepo.All().FirstOrDefault(x => x.SpecificationTypeId == c.Id).Description
        }).ToList();

        return result;
    }

    public MaterialTemplateEditDto Get(int typeId)
    {
        var dto = new MaterialTemplateEditDto();
        var type = _stRepo.Get(typeId);
        if (_mtRepo.All().Any(c => c.SpecificationTypeId == typeId))
        {
            var template = _mtRepo.All().FirstOrDefault(c => c.SpecificationTypeId == typeId);
            dto.Id = template.Id;
            dto.TypeId = typeId;
            dto.TypeName = type.Name;
            dto.Description = template.Description;
        }
        else
        {
            dto.TypeId = typeId;
            dto.TypeName = type.Name;
        }

        return dto;
    }

    public bool Update(MaterialTemplateEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        var isEdit = dto.Id > 0;
        var template = new MaterialTemplate();
        if (isEdit)
        {
            template = _mtRepo.Get(dto.Id);
            template.Description = dto.Description;
        }
        else
        {
            template.SpecificationTypeId = dto.TypeId;
            template.Description = dto.Description;
        }

        if (isEdit)
            _mtRepo.Update(template);
        else
            _mtRepo.Add(template);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;

        return result;
    }

    private string getDescription(int typeId)
    {
        var result = "";
        if (_mtRepo.All().Any(c => c.SpecificationTypeId == typeId))
        {
            var template = _mtRepo.All().FirstOrDefault(c => c.SpecificationTypeId == typeId);
            result = template.Description;
        }

        return result;
    }
}