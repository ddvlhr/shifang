using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.Report;
using Core.Entities;
using Infrastructure.DataBase;

namespace Infrastructure.Services.Reports.Impl;

public class InspectionReportAppearanceService : IInspectionReportAppearanceService
{
    private readonly IRepository<InspectionReportAppearance> _iraRepo;
    private readonly IRepository<InspectionReport> _irRepo;
    private readonly IUnitOfWork _uow;

    public InspectionReportAppearanceService(IRepository<InspectionReportAppearance> iraRepo,
        IRepository<InspectionReport> irRepo, IUnitOfWork uow)
    {
        _iraRepo = iraRepo;
        _irRepo = irRepo;
        _uow = uow;
    }

    public bool Update(EditReportAppearanceDto dto, out string failReason)
    {
        failReason = string.Empty;
        var inDb = _iraRepo.All().Where(c => c.ReportId == dto.ReportId).ToList();
        var inDbIds = inDb.Select(c => c.IndicatorId).ToList();
        var addList = new List<InspectionReportAppearance>();
        var updateList = new List<InspectionReportAppearance>();
        var updateIds = new List<int>();
        var deleteList = new List<InspectionReportAppearance>();

        if (dto.Appearances.Count() > 0)
        {
            var report = _irRepo.Get(dto.ReportId);
            report.FinalRet = (ReportRet)dto.FinalRet;

            foreach (var item in dto.Appearances)
                if (item.DbId == 0)
                {
                    var data = new InspectionReportAppearance
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
                    var data = _iraRepo.Get(item.DbId);
                    data.IndicatorId = item.IndicatorId;
                    data.Frequency = Convert.ToInt32(item.Frequency);
                    data.SubScore = Convert.ToInt32(item.SubScore);
                    updateList.Add(data);
                }

            var deleteIds = inDbIds.Where(c => !updateIds.Contains(c)).ToList();
            deleteList = _iraRepo.All().Where(c => deleteIds.Contains(c.Id)).ToList();
        }
        else
        {
            deleteList = inDb;
        }

        _iraRepo.AddRange(addList);
        _iraRepo.UpdateRange(updateList);
        _iraRepo.DeleteRange(deleteList);

        return _uow.Save() >= 0;
    }

    public EditReportAppearanceDto GetReportAppearances(int id)
    {
        var report = _irRepo.Get(id);
        var appearances = _iraRepo.All().Where(c => c.ReportId == id).Select(c =>
            new EditReportAppearanceDto.AppearanceInfo
            {
                DbId = c.Id, IndicatorId = c.IndicatorId, Frequency = c.Frequency.ToString(),
                SubScore = c.SubScore.ToString()
            }).ToList();

        var appSubScore = appearances.Sum(c => Convert.ToInt32(c.SubScore));
        var dto = new EditReportAppearanceDto
        {
            ReportId = id,
            Total = 100 - report.PhyRetDeduction - appSubScore,
            FinalRet = Convert.ToInt32(report.FinalRet),
            Appearances = appearances
        };

        return dto;
    }

    public int GetScore(int id)
    {
        var data = _iraRepo.All().Where(c => c.ReportId == id);
        var score = 0;
        foreach (var item in data) score += item.SubScore * item.Frequency;

        return score;
    }
}