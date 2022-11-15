using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.Specification;
using Core.Dtos.MetricalData;
using Core.Dtos.Report;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Helper;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services.Reports.Impl;

public class CraftReportService : ICraftReportService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IRepository<CraftIndicatorRule> _cirRepo;
    private readonly IRepository<CraftReport> _crRepo;
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<MeasureTypeIndicators> _mtiRepo;
    private readonly IRepository<Role> _rRepo;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;
    private readonly IUnitOfWork _uow;
    private readonly IUserService _userService;

    public CraftReportService(IRepository<CraftReport> crRepo, IRepository<Group> gRepo,
        IRepository<Data> dRepo, IRepository<Core.Entities.Specification> spRepo,
        IRepository<CraftIndicatorRule> cirRepo, IRepository<MeasureTypeIndicators> mtiRepo,
        IRepository<Core.Entities.Indicator> iRepo, IUnitOfWork uow, IOptionsSnapshot<Settings> settings,
        IUserService userService, IHttpContextAccessor accessor, IRepository<Role> rRepo)
    {
        _crRepo = crRepo;
        _gRepo = gRepo;
        _dRepo = dRepo;
        _spRepo = spRepo;
        _cirRepo = cirRepo;
        _mtiRepo = mtiRepo;
        _iRepo = iRepo;
        _uow = uow;
        _userService = userService;
        _accessor = accessor;
        _rRepo = rRepo;
        _settings = settings.Value;
    }

    public IEnumerable<ReportTableDto> GetTable(QueryInfoDto dto, out int total)
    {
        var data = _crRepo.All().AsNoTracking();
        var roleId = _accessor.HttpContext.getUserRoleId();
        var canSeeOtherData = _accessor.HttpContext.getCanSeeOtherData();
        if (roleId != _settings.AdminTypeId)
            if (!canSeeOtherData)
            {
                var userId = _accessor.HttpContext.getUserId();
                data = data.Where(c => c.Group.UserId == userId || c.Group.UserId == 0);
            }

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = Convert.ToInt32(dto.SpecificationId);
            data = data.Where(c => c.Group.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var typeId = Convert.ToInt32(dto.SpecificationTypeId);
            data = data.Where(c => c.Group.Specification.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = Convert.ToInt32(dto.TurnId);
            data = data.Where(c => c.Group.TurnId == turnId);
        }

        if (!string.IsNullOrEmpty(dto.MachineModelId))
        {
            var machineModelId = Convert.ToInt32(dto.MachineModelId);
            data = data.Where(c => c.Group.MachineModelId == machineModelId);
        }

        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c => c.Group.Specification.Name.Contains(dto.Query) ||
                                   c.Group.Specification.SpecificationType.Name.Contains(dto.Query) ||
                                   c.Group.Turn.Name.Contains(dto.Query) ||
                                   c.Group.MachineModel.Name.Contains(dto.Query) ||
                                   c.Group.UserName.Contains(dto.Query));

        total = data.Count();

        var result = data.OrderByDescending(c => c.Group.BeginTime).Include(c => c.Group)
            .ThenInclude(c => c.MachineModel).ThenInclude(c => c.Model)
            .Include(c => c.Group).ThenInclude(c => c.Specification)
            .Include(c => c.Group).ThenInclude(c => c.Turn).Skip(dto.Skip()).Take(dto.PageSize).Select(c => new
                ReportTableDto
                {
                    Id = c.Id, BeginTime = c.Group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProductDate = c.Group.ProductionTime == null
                        ? ""
                        : Convert.ToDateTime(c.Group.ProductionTime).ToString("yyyy-MM-dd"),
                    SpecificationId = c.Group.SpecificationId,
                    SpecificationName = c.Group.Specification.Name, TurnName = c.Group.Turn.Name,
                    MachineModelName = c.Group.MachineModel.Name,
                    ModelName = c.Group.MachineModel.Model.Name,
                    FinalRet = (int)c.ReportRet,
                    UserName = c.Group.UserName
                }).ToList();

        return result;
    }

    public bool Add(MetricalDataEditDataDto dto, out string failReason)
    {
        failReason = string.Empty;
        var report = new CraftReport
        {
            GroupId = dto.GroupId
        };
        var isEdit = false;
        if (_crRepo.All().Any(c => c.GroupId == dto.GroupId))
        {
            isEdit = true;
            report = _crRepo.All().FirstOrDefault(c => c.GroupId == dto.GroupId);
        }

        if (report == null) return false;

        var group = _gRepo.Get(dto.GroupId);
        report.OrderNo = group.OrderNo;
        report.ReportRet = ReportRet.未完成;
        report.LogOrderNo = _settings.CraftLogOrderNo;

        if (isEdit)
            _crRepo.Update(report);
        else
            _crRepo.Add(report);

        var result = _uow.Save() >= 0;
        return result;
    }

    public bool Edit(CraftReportEditDto dto)
    {
        var report = _crRepo.Get(dto.Id);
        report.OrderNo = dto.OrderNo;
        report.Score = dto.Score;
        report.Remark = dto.Remark;
        report.ReportRet = dto.State ? ReportRet.合格 : ReportRet.不合格;
        report.Temperature = dto.Temperature;
        report.ControlSituation = dto.ControlSituation;
        report.LogOrderNo = dto.LogOrderNo;

        _crRepo.Update(report);
        var result = _uow.Save() >= 0;

        return result;
    }

    public CraftReportEditDto GetInfo(int id)
    {
        var dto = new CraftReportEditDto();
        var report = _crRepo.All().Include(c => c.Group).ThenInclude(c => c.MachineModel).ThenInclude(c => c.Model)
            .Include(c => c.Group).ThenInclude(c => c.Specification)
            .FirstOrDefault(c => c.Id == id);
        if (report == null) return dto;

        dto.Id = report.Id;
        dto.SpecificationName = report.Group.Specification.Name;
        dto.MachineModelName = report.Group.MachineModel.Name;
        dto.ModelName = report.Group.MachineModel.Model.Name;
        dto.OrderNo = report.OrderNo;
        dto.Score = report.Score;
        dto.Remark = report.Remark;
        dto.State = report.ReportRet == ReportRet.合格;
        dto.Temperature = report.Temperature;
        dto.ControlSituation = report.ControlSituation;
        dto.LogOrderNo = report.LogOrderNo;

        return dto;
    }

    public CraftReportInfoDto GetReportInfo(int id)
    {
        var dto = new CraftReportInfoDto();
        var report = _crRepo.All().Include(c => c.Group).ThenInclude(c => c.Specification).Include(c => c.Group)
            .ThenInclude(c => c.MachineModel).ThenInclude(c => c.Model).Include(c => c.Group)
            .ThenInclude(c => c.MachineModel).ThenInclude(c => c.WorkShop).Include(c => c.Group)
            .ThenInclude(c => c.Turn).FirstOrDefault(c => c
                .Id == id);
        if (report == null) return dto;

        var user =
            dto.WorkShopName = report.Group.MachineModel.WorkShop.Name;
        dto.TestDate = report.Group.BeginTime.ToString("yyyy年MM月dd日");
        dto.ModelName = report.Group.MachineModel.Model.Name;
        dto.OrderNo = report.OrderNo;
        dto.Score = report.Score.ToString();
        dto.SpecificationName = report.Group.Specification.Name;
        dto.TemperatureInfo = report.Temperature;
        dto.ControlInfo = report.ControlSituation;
        dto.TurnName = report.Group.Turn.Name;
        dto.LogOrderNo = report.LogOrderNo;
        dto.UserName = _userService.GetUserName(_settings.CraftManager);
        var craftIndicator = _cirRepo.All().FirstOrDefault(c => c.ModelId == report.Group.MachineModel.ModelId);
        if (craftIndicator == null) return dto;

        var rules = JsonConvert.DeserializeObject<List<Rule>>(craftIndicator.Rules);
        var data = _dRepo.All().FirstOrDefault(c => c.GroupId == report.GroupId);
        if (data != null)
        {
            var dataObj = JsonConvert.DeserializeObject<JObject>(data.Data);
            var items = new List<CraftReportInfoDto.CraftTestItem>
            {
                new()
                {
                    MachineModelName = report.Group.MachineModel.Name,
                    Name = "生产品牌",
                    Type = "类别",
                    Value = report.Group.Specification.Name
                }
            };
            foreach (var rule in rules)
            {
                var indicator = _iRepo.All().Include(c => c.IndicatorParent).FirstOrDefault(c => c.Id == rule.Id);
                if (indicator == null) continue;

                var item = new CraftReportInfoDto.CraftTestItem
                {
                    MachineModelName = report.Group.MachineModel.Name,
                    Name = indicator.Name,
                    Type = indicator.IndicatorParent.Name
                };

                var obj = dataObj[$"{rule.Id}"];
                if (obj == null)
                    item.Value = "";
                else
                    item.Value = obj.ToString();

                items.Add(item);
            }

            dto.CraftTestItems = items;
        }


        return dto;
    }
}