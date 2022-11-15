using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.Report;
using Core.Entities;
using Infrastructure.DataBase;

namespace Infrastructure.Services.Reports.Impl;

public class ProductReportAppearanceService : IProductReportAppearanceService
{
    private readonly IRepository<ProductReportAppearance> _praRepo;
    private readonly IUnitOfWork _uow;

    public ProductReportAppearanceService(IRepository<ProductReportAppearance> praRepo, IUnitOfWork uow)
    {
        _praRepo = praRepo;
        _uow = uow;
    }

    public bool Update(EditReportAppearanceDto dto, out string failReason)
    {
        failReason = string.Empty;
        var inDb = _praRepo.All().Where(c => c.ReportId == dto.ReportId).Select(c => c.IndicatorId).ToList();
        var addList = new List<ProductReportAppearance>();
        var updateList = new List<ProductReportAppearance>();
        var updateIds = new List<int>();

        foreach (var item in dto.Appearances)
            if (item.DbId == 0)
            {
                var data = new ProductReportAppearance
                {
                    ReportId = dto.ReportId,
                    IndicatorId = item.IndicatorId,
                    Frequency = Convert.ToInt32(item.Frequency),
                    SubScore = Convert.ToInt32(item.SubScore)
                };

                addList.Add(data);
            }
            else
            {
                updateIds.Add(item.DbId);
                var data = _praRepo.Get(item.DbId);
                data.IndicatorId = item.IndicatorId;
                data.Frequency = Convert.ToInt32(item.Frequency);
                data.SubScore = Convert.ToInt32(item.SubScore);
                updateList.Add(data);
            }

        var deleteIds = inDb.Where(c => !updateIds.Contains(c)).ToList();
        var deleteList = _praRepo.All().Where(c => deleteIds.Contains(c.Id)).ToList();

        _praRepo.AddRange(addList);
        _praRepo.UpdateRange(updateList);
        _praRepo.DeleteRange(deleteList);

        return _uow.Save() >= 0;
    }

    public EditReportAppearanceDto GetReportAppearances(int id)
    {
        var appearances = _praRepo.All().Where(c => c.ReportId == id).Select(c =>
            new EditReportAppearanceDto.AppearanceInfo
            {
                DbId = c.Id, IndicatorId = c.IndicatorId, Frequency = c.Frequency.ToString(),
                SubScore = c.SubScore.ToString()
            }).ToList();

        var dto = new EditReportAppearanceDto
        {
            ReportId = id,
            Appearances = appearances
        };

        return dto;
    }

    public int GetScore(int id)
    {
        var data = _praRepo.All().Where(c => c.ReportId == id).Select(c => new { c.SubScore, c.Frequency }).ToList();

        return data.Sum(item => item.SubScore * item.Frequency);
    }
}