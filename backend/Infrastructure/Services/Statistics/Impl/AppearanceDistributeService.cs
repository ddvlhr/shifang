using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Dtos.AppearanceDistribute;
using Core.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Statistics.Impl;

public class AppearanceDistributeService : IAppearanceDistributeService
{
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<IndicatorParent> _ipRepo;
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<InspectionReport> _irRepo;
    private readonly IRepository<MachineModel> _mmRepo;
    private readonly IRepository<PhysicalReportAppearance> _praRepo;
    private readonly IRepository<ProductReport> _prodRepo;
    private readonly IRepository<PhysicalReport> _prRepo;
    private readonly IRepository<Core.Entities.Specification> _sRepo;
    private readonly IRepository<Turn> _tRepo;

    public AppearanceDistributeService(IRepository<Group> gRepo, IRepository<Data> dRepo,
        IRepository<PhysicalReport> prRepo,
        IRepository<InspectionReport> irRepo, IRepository<ProductReport> prodRepo,
        IRepository<Core.Entities.Specification> sRepo,
        IRepository<Turn> tRepo, IRepository<MachineModel> mmRepo, IRepository<PhysicalReportAppearance> praRepo,
        IRepository<IndicatorParent> ipRepo, IRepository<Core.Entities.Indicator> iRepo)
    {
        _gRepo = gRepo;
        _dRepo = dRepo;
        _prRepo = prRepo;
        _irRepo = irRepo;
        _prodRepo = prodRepo;
        _sRepo = sRepo;
        _tRepo = tRepo;
        _mmRepo = mmRepo;
        _praRepo = praRepo;
        _iRepo = iRepo;
        _ipRepo = ipRepo;
    }

    public IEnumerable<AppearanceDataDto> Search(DateTime beginDate, DateTime endDate, int typeId)
    {
        throw new NotImplementedException();
    }

    public MemoryStream Download(DateTime beginDate, DateTime endDate, int typeId)
    {
        throw new NotImplementedException();
    }

    private IEnumerable<AppearanceDataDto> getData(DateTime beginDate, DateTime endDate, int typeId)
    {
        var data = (from report in _irRepo.All().AsNoTracking()
            where report.FinalRet != ReportRet.未完成
            join groups in _gRepo.All().AsNoTracking() on report.GroupId equals groups.Id
            where groups.BeginTime.Date >= beginDate && groups.EndTime.Date <= endDate
            join specification in _sRepo.All().AsNoTracking() on groups.SpecificationId equals specification.Id
            join turn in _tRepo.All().AsNoTracking() on groups.TurnId equals turn.Id
            join machineModel in _mmRepo.All().AsNoTracking() on groups.MachineModelId equals machineModel.Id
            join physicalReportAppearance in _praRepo.All().AsNoTracking() on report.Id equals
                physicalReportAppearance.ReportId into pra
            from physicalReportAppearance in pra.DefaultIfEmpty()
            join indicator in _iRepo.All().AsNoTracking() on physicalReportAppearance.IndicatorId equals
                indicator.Id into indicators
            from indicator in indicators.DefaultIfEmpty()
            join indicatorParent in _ipRepo.All().AsNoTracking() on indicator.IndicatorParentId equals
                indicatorParent.Id into indicatorParents
            from indicatorParent in indicatorParents.DefaultIfEmpty()
            select new AppearanceDataDto
            {
                GroupId = groups.Id, SpecificationId = specification.Id, SpecificationName = specification.Name,
                TurnId = turn.Id, TurnName = turn.Name, MachineModelId = machineModel.Id,
                MachineModelName = machineModel.Name
            }).ToList();

        return data;
    }
}