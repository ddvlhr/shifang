using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.WorkShopQuality;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.BaseData.Impl;

public class WorkShopQualityPointRuleService : IWorShopQualityPointRuleService
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<WorkShopQualityPointRule> _wsqrRepo;

    public WorkShopQualityPointRuleService(IRepository<WorkShopQualityPointRule> wsqrRepo, IUnitOfWork uow)
    {
        _wsqrRepo = wsqrRepo;
        _uow = uow;
    }

    public IEnumerable<WorkShopQualityPointRuleTableDto> GetTable(WorkShopQualityPointRuleQueryInfoDto dto,
        out int total)
    {
        var data = _wsqrRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.WorkShopId))
        {
            var workShopId = int.Parse(dto.WorkShopId);
            data = data.Where(c => c.WorkShopId == workShopId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var specificationTypeId = int.Parse(dto.SpecificationTypeId);
            data = data.Where(c => c.SpecificationTypeId == specificationTypeId);
        }

        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c => c.WorkShop.Name.Contains(dto.Query) ||
                                   c.SpecificationType.Name.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.State))
        {
            var state = int.Parse(dto.State);
            data = data.Where(c => c.Status == (Status)state);
        }

        total = data.Count();

        var list = data.OrderByDescending(c => c.ModifiedAtUtc).Include(c => c.SpecificationType)
            .Include(c => c.WorkShop).Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new WorkShopQualityPointRuleTableDto
                {
                    Id = c.Id, WorkShopName = c.WorkShop.Name, SpecificationTypeName = c.SpecificationType.Name,
                    State = c.Status == Status.Enabled
                }).ToList();
        return list;
    }

    public bool Add(WorkShopQualityPointRuleEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_wsqrRepo.All().Any(c => c.WorkShopId == dto.WorkShopId &&
                                     c.SpecificationTypeId == dto.SpecificationTypeId))
        {
            failReason = "该车间和牌号类型以经设置了规则, 请设置其他类型";
            return false;
        }

        var data = new WorkShopQualityPointRule
        {
            WorkShopId = dto.WorkShopId,
            SpecificationTypeId = dto.SpecificationTypeId,
            PhysicalPointsPercent = Convert.ToInt32(dto.PhysicalPointPercent),
            PhysicalAllPercent = Convert.ToInt32(dto.PhysicalAllPercent),
            InspectionPointsPercent = Convert.ToInt32(dto.InspectionPointPercent),
            InspectionAllPercent = Convert.ToInt32(dto.InspectionAllPercent),
            ProductAllPercent = Convert.ToInt32(dto.ProductAllPercent),
            ProductPointsPercent = Convert.ToInt32(dto.ProductPointPercent),
            Status = dto.State ? Status.Enabled : Status.Disabled
        };

        _wsqrRepo.Add(data);

        var result = _uow.Save() > 0;

        return result;
    }

    public bool Edit(WorkShopQualityPointRuleEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_wsqrRepo.All().Any(c => c.Id != dto.Id &&
                                     c.WorkShopId == dto.WorkShopId &&
                                     c.SpecificationTypeId == dto.SpecificationTypeId))
        {
            failReason = "该车间和牌号类型以经设置了规则, 请设置其他类型";
            return false;
        }

        var data = _wsqrRepo.Get(dto.Id);
        data.WorkShopId = dto.WorkShopId;
        data.SpecificationTypeId = dto.SpecificationTypeId;
        data.PhysicalAllPercent = Convert.ToInt32(dto.PhysicalAllPercent);
        data.PhysicalPointsPercent = Convert.ToInt32(dto.PhysicalPointPercent);
        data.InspectionAllPercent = Convert.ToInt32(dto.InspectionAllPercent);
        data.InspectionPointsPercent = Convert.ToInt32(dto.InspectionPointPercent);
        data.ProductAllPercent = Convert.ToInt32(dto.ProductAllPercent);
        data.ProductPointsPercent = Convert.ToInt32(dto.ProductPointPercent);
        data.Status = dto.State ? Status.Enabled : Status.Disabled;

        _wsqrRepo.Update(data);

        var result = _uow.Save() >= 0;
        return result;
    }

    public bool Delete(IEnumerable<int> ids)
    {
        var data = _wsqrRepo.All().Where(c => ids.Contains(c.Id)).ToList();

        _wsqrRepo.DeleteRange(data);

        var result = _uow.Save() >= 0;

        return result;
    }

    public WorkShopQualityPointRuleEditDto GetInfo(int id)
    {
        var dto = new WorkShopQualityPointRuleEditDto();

        var data = _wsqrRepo.All().Include(c => c.WorkShop).Include(c => c.SpecificationType)
            .FirstOrDefault(c => c.Id == id);
        if (data == null) return dto;

        dto.Id = data.Id;
        dto.WorkShopId = data.WorkShopId;
        dto.SpecificationTypeId = data.SpecificationTypeId;
        dto.PhysicalPointPercent = data.PhysicalPointsPercent.ToString();
        dto.PhysicalAllPercent = data.PhysicalAllPercent.ToString();
        dto.InspectionPointPercent = data.InspectionPointsPercent.ToString();
        dto.InspectionAllPercent = data.InspectionAllPercent.ToString();
        dto.ProductPointPercent = data.ProductPointsPercent.ToString();
        dto.ProductAllPercent = data.ProductAllPercent.ToString();
        dto.State = data.Status == Status.Enabled;

        return dto;
    }
}