using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.Report;
using Core.Entities;
using Infrastructure.DataBase;

namespace Infrastructure.Services.Reports.Impl
{
    public class TestReportAppearanceService: ITestReportAppearanceService
    {
        private readonly IRepository<TestReportAppearance> _traRepo;
        private readonly IUnitOfWork _uow;

        public TestReportAppearanceService(IRepository<TestReportAppearance> traRepo,
            IUnitOfWork uow)
        {
            _traRepo = traRepo;
            _uow = uow;
        }
        public bool Update(EditReportAppearanceDto dto, out string failReason)
    {
        failReason = string.Empty;
        var inDb = _traRepo.All().Where(c => c.ReportId == dto.ReportId).Select(c => c.IndicatorId).ToList();
        var addList = new List<TestReportAppearance>();
        var updateList = new List<TestReportAppearance>();
        var updateIds = new List<int>();

        foreach (var item in dto.Appearances)
            if (item.DbId == 0)
            {
                var data = new TestReportAppearance
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
                var data = _traRepo.Get(item.DbId);
                data.IndicatorId = item.IndicatorId;
                data.Frequency = Convert.ToInt32(item.Frequency);
                data.SubScore = Convert.ToInt32(item.SubScore);
                updateList.Add(data);
            }

        var deleteIds = inDb.Where(c => !updateIds.Contains(c)).ToList();
        var deleteList = _traRepo.All().Where(c => deleteIds.Contains(c.Id)).ToList();

        _traRepo.AddRange(addList);
        _traRepo.UpdateRange(updateList);
        _traRepo.DeleteRange(deleteList);

        return _uow.Save() >= 0;
    }

    public EditReportAppearanceDto GetReportAppearances(int id)
    {
        var appearances = _traRepo.All().Where(c => c.ReportId == id).Select(c =>
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
        var data = _traRepo.All().Where(c => c.ReportId == id).Select(c => new { c.SubScore, c.Frequency }).ToList();

        return data.Sum(item => item.SubScore * item.Frequency);
    }
    }
}