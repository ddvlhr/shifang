using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Entities;
using Infrastructure.DataBase;
using Infrastructure.Services.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class SpecificationTypeService : ISpecificationTypeService
{
    private const string _reason = "该牌号类型名称已被使用, 请使用其他名称";
    private readonly ISpecificationService _spService;
    private readonly IRepository<SpecificationType> _stRepo;
    private readonly IUnitOfWork _uow;

    public SpecificationTypeService(IRepository<SpecificationType> stRepo, ISpecificationService spService,
        IUnitOfWork uow)
    {
        _stRepo = stRepo;
        _spService = spService;
        _uow = uow;
    }

    public IEnumerable<SpecificationTypeTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _stRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.Query)) data = data.Where(c => c.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new SpecificationTypeTableDto
                {
                    Id = c.Id, Name = c.Name,
                    Remark = c.Remark,
                    ProductOrderNo = c.ProductOrderNo,
                    InspectionOrderNo = c.InspectionOrderNo,
                    PhysicalOrderNo = c.PhysicalOrderNo,
                    MaterialOrderNo = c.MaterialOrderNo,
                    FactoryOrderNo = c.FactoryOrderNo,
                    CraftOrderNo = c.CraftOrderNo,
                    State = c.Status == Status.Enabled
                }).ToList();

        return result;
    }

    public bool Add(SpecificationTypeEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_stRepo.All().Any(c => c.Name == dto.Name))
        {
            failReason = _reason;
            return false;
        }

        var specificationType = new SpecificationType
        {
            Name = dto.Name,
            Remark = dto.Remark,
            ProductOrderNo = dto.ProductOrderNo,
            InspectionOrderNo = dto.InspectionOrderNo,
            PhysicalOrderNo = dto.PhysicalOrderNo,
            MaterialOrderNo = dto.MaterialOrderNo,
            CraftOrderNo = dto.CraftOrderNo,
            FactoryOrderNo = dto.FactoryOrderNo,
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _stRepo.Add(specificationType);

        return _uow.Save() > 0;
    }

    public bool Update(SpecificationTypeEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_stRepo.All().Any(c => c.Id != dto.Id && c.Name == dto.Name))
        {
            failReason = _reason;
            return false;
        }

        var specificationType = _stRepo.Get(dto.Id);
        specificationType.Name = dto.Name;
        specificationType.Remark = dto.Remark;
        specificationType.ProductOrderNo = dto.ProductOrderNo;
        specificationType.InspectionOrderNo = dto.InspectionOrderNo;
        specificationType.PhysicalOrderNo = dto.PhysicalOrderNo;
        specificationType.MaterialOrderNo = dto.MaterialOrderNo;
        specificationType.CraftOrderNo = dto.CraftOrderNo;
        specificationType.FactoryOrderNo = dto.FactoryOrderNo;
        specificationType.Status =
            dto.State ? Status.Enabled : Status.Disabled;

        _stRepo.Update(specificationType);

        return _uow.Save() >= 0;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _stRepo.All().Where(c => c.Status == Status.Enabled).OrderBy(c => c.Id).Select(c =>
            new BaseOptionDto
            {
                Value = c.Id, Text = c.Name
            }).ToList();

        return data;
    }

    public SpecificationTypeInfoDto GetSpecificationTypeInfo(int id)
    {
        var specificationType = _stRepo.Get(id);
        var dto = new SpecificationTypeInfoDto
        {
            OrderNo = specificationType.Remark,
            ProductOrderNo = specificationType.ProductOrderNo,
            InspectionOrderNo = specificationType.InspectionOrderNo,
            PhysicalOrderNo = specificationType.PhysicalOrderNo,
            MaterialOrderNo = specificationType.MaterialOrderNo,
            CraftOrderNo = specificationType.CraftOrderNo
        };

        var specifications = _spService.GetOptions();
        dto.Specifications = specifications;

        return dto;
    }
}