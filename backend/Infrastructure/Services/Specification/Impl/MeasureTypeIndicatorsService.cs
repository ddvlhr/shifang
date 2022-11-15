using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Specification.Impl;

public class MeasureTypeIndicatorsService : IMeasureTypeIndicatorsService
{
    private readonly IRepository<MeasureTypeIndicators> _mtiRepo;
    private readonly IRepository<MeasureType> _mtRepo;
    private readonly IUnitOfWork _uow;

    public MeasureTypeIndicatorsService(IRepository<MeasureTypeIndicators> mtiRepo, IRepository<MeasureType> mtRepo,
        IUnitOfWork uow)
    {
        _mtiRepo = mtiRepo;
        _mtRepo = mtRepo;
        _uow = uow;
    }

    public IEnumerable<MeasureTypeIndicatorTableDto> GetTable(BaseQueryInfoDto dto, out int total)
    {
        var data = _mtRepo.All().AsNoTracking().Where(c => c.Status == Status.Enabled);
        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c =>
                c.Name.Contains(dto.Query));

        total = data.Count();
        var result = data.OrderByDescending(c => c.ModifiedAtUtc)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new MeasureTypeIndicatorTableDto
                {
                    Id = c.Id, Name = c.Name,
                    Count = _mtiRepo.All()
                        .Count(x => x.MeasureTypeId == c.Id)
                }).ToList();

        return result;
    }

    public bool Update(MeasureTypeIndicatorsEditDto dto,
        out string failReason)
    {
        failReason = string.Empty;
        var inDbIndicators = _mtiRepo.All().Where(c => c.MeasureTypeId == dto.Id).ToList();
        var insertData = new List<MeasureTypeIndicators>();
        var updateData = new List<MeasureTypeIndicators>();
        var inDbIds = inDbIndicators.Select(c => c.Id).ToList();
        var nowIds = new List<int>();
        foreach (var rule in dto.IndicatorRules)
            if (inDbIndicators.Count == 0)
            {
                var data = new MeasureTypeIndicators
                {
                    MeasureTypeId = dto.Id,
                    IndicatorId = rule.Id,
                    SpecificationTypeId = rule.SpecificationTypeId,
                    Points = Convert.ToInt32(rule.Points),
                    Deduction = Convert.ToInt32(rule.Deduction),
                    MeanPoints = Convert.ToInt32(rule.MeanPoints),
                    MeanDeduction = Convert.ToInt32(rule.MeanDeduction),
                    SdPoints = Convert.ToInt32(rule.SdPoints),
                    SdDeduction = Convert.ToInt32(rule.SdDeduction),
                    CvPoints = Convert.ToInt32(rule.CvPoints),
                    CvDeduction = Convert.ToInt32(rule.CvDeduction),
                    UnQualifiedCount = Convert.ToInt32(rule.UnQualifiedCount),
                    UnQualifiedOperator = (UnQualifiedOperator)rule.UnQualifiedOperator
                };
                insertData.Add(data);
            }
            else
            {
                if (rule.DbId == 0)
                {
                    var data = new MeasureTypeIndicators
                    {
                        MeasureTypeId = dto.Id,
                        IndicatorId = rule.Id,
                        SpecificationTypeId = rule.SpecificationTypeId,
                        Points = Convert.ToInt32(rule.Points),
                        Deduction = Convert.ToInt32(rule.Deduction),
                        MeanPoints = Convert.ToInt32(rule.MeanPoints),
                        MeanDeduction = Convert.ToInt32(rule.MeanDeduction),
                        SdPoints = Convert.ToInt32(rule.SdPoints),
                        SdDeduction = Convert.ToInt32(rule.SdDeduction),
                        CvPoints = Convert.ToInt32(rule.CvPoints),
                        CvDeduction = Convert.ToInt32(rule.CvDeduction),
                        UnQualifiedCount = Convert.ToInt32(rule.UnQualifiedCount),
                        UnQualifiedOperator = (UnQualifiedOperator)rule.UnQualifiedOperator
                    };
                    insertData.Add(data);
                }
                else
                {
                    nowIds.Add(rule.DbId);
                    var data =
                        inDbIndicators.FirstOrDefault(
                            c => c.Id == rule.DbId);
                    data.IndicatorId = rule.Id;
                    data.SpecificationTypeId = rule.SpecificationTypeId;
                    data.Points = Convert.ToInt32(rule.Points);
                    data.Deduction = Convert.ToInt32(rule.Deduction);
                    data.MeanPoints = Convert.ToInt32(rule.MeanPoints);
                    data.MeanDeduction =
                        Convert.ToInt32(rule.MeanDeduction);
                    data.SdPoints = Convert.ToInt32(rule.SdPoints);
                    data.SdDeduction = Convert.ToInt32(rule.SdDeduction);
                    data.CvPoints = Convert.ToInt32(rule.CvPoints);
                    data.CvDeduction = Convert.ToInt32(rule.CvDeduction);
                    data.UnQualifiedCount = Convert.ToInt32(rule.UnQualifiedCount);
                    data.UnQualifiedOperator = (UnQualifiedOperator)rule.UnQualifiedOperator;
                    updateData.Add(data);
                }
            }

        var removeIds = inDbIds.Where(c => !nowIds.Contains(c)).ToList();
        if (removeIds.Count > 0)
        {
            var removeData = _mtiRepo.All().Where(c => removeIds.Contains(c.Id))
                .ToList();
            _mtiRepo.DeleteRange(removeData);
        }

        _mtiRepo.AddRange(insertData);
        _mtiRepo.UpdateRange(updateData);

        return _uow.Save() >= 0;
    }

    public MeasureTypeIndicatorsEditDto GetInfo(int id)
    {
        var data = _mtiRepo.All().Where(c => c.MeasureTypeId == id).Select(
            c => new MeasureTypeIndicatorsEditDto.IndicatorRule
            {
                DbId = c.Id, Id = c.IndicatorId,
                SpecificationTypeId = c.SpecificationTypeId,
                Points = c.Points.ToString(),
                Deduction = c.Deduction.ToString(),
                MeanPoints = c.MeanPoints.ToString(),
                MeanDeduction = c.MeanDeduction.ToString(),
                SdPoints = c.SdPoints.ToString(),
                SdDeduction = c.SdDeduction.ToString(),
                CvPoints = c.CvPoints.ToString(),
                CvDeduction = c.CvDeduction.ToString(),
                UnQualifiedCount = c.UnQualifiedCount.ToString(),
                UnQualifiedOperator = Convert.ToInt32(c.UnQualifiedOperator)
            }).ToList();

        var result = new MeasureTypeIndicatorsEditDto
        {
            Id = id,
            IndicatorRules = data
        };

        return result;
    }
}