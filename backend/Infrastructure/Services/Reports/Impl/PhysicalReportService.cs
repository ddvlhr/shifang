using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Dtos.Specification;
using Core.Dtos.MetricalData;
using Core.Dtos.Report;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Infrastructure.Helper;
using Infrastructure.Services.MetricalData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services.Reports.Impl;

public class PhysicalReportService : IPhysicalReportService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<MeasureTypeIndicators> _mtiRepo;
    private readonly IRepository<PhysicalReportAppearance> _praRepo;
    private readonly IRepository<PhysicalReport> _prRepo;
    private readonly IRepository<Role> _rRepo;
    private readonly IServiceProvider _services;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;
    private readonly IUnitOfWork _uow;

    public PhysicalReportService(IRepository<PhysicalReport> prRepo,
        IRepository<PhysicalReportAppearance> praRepo,
        IServiceProvider services, IRepository<Group> gRepo,
        IRepository<Core.Entities.Specification> spRepo,
        IRepository<MeasureTypeIndicators> mtiRepo, IUnitOfWork uow, IRepository<Role> rRepo,
        IHttpContextAccessor accessor, IOptionsSnapshot<Settings> settings)
    {
        _prRepo = prRepo;
        _praRepo = praRepo;
        _services = services;
        _gRepo = gRepo;
        _spRepo = spRepo;
        _mtiRepo = mtiRepo;
        _uow = uow;
        _rRepo = rRepo;
        _accessor = accessor;
        _settings = settings.Value;
    }

    public bool Add(AddReportDto dto)
    {
        var report = new PhysicalReport();
        var isEdit = false;
        if (_prRepo.All().Any(c => c.GroupId == dto.GroupId))
        {
            isEdit = true;
            report = _prRepo.All()
                .FirstOrDefault(c => c.GroupId == dto.GroupId);
            report.Water = dto.Water;
            report.Humidity = dto.Humidity;
            report.Temperature = dto.Temperature;
            report.PhyRet = (ReportRet)dto.PhyRet;
            report.PhyRetDes = dto.PhyRetDes;
            report.Remark = dto.Remark;
            report.FinalRet = (ReportRet)dto.FinalRet;
        }
        else
        {
            report.GroupId = dto.GroupId;
            report.Water = dto.Water;
            report.Humidity = dto.Humidity;
            report.Temperature = dto.Temperature;
            report.PhyRet = (ReportRet)dto.PhyRet;
            report.PhyRetDes = dto.PhyRetDes;
            report.Remark = dto.Remark;
            report.FinalRet = (ReportRet)dto.FinalRet;
        }

        if (isEdit)
            _prRepo.Update(report);
        else
            _prRepo.Add(report);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;

        return result;
    }

    public bool Add(MetricalDataEditDataDto dto)
    {
        var report = new PhysicalReport
        {
            GroupId = dto.GroupId
        };
        var isEdit = false;
        if (_prRepo.All().Any(c => c.GroupId == dto.GroupId))
        {
            isEdit = true;
            report = _prRepo.All().FirstOrDefault(c => c.GroupId == dto.GroupId);
        }

        var group = _gRepo.Get(dto.GroupId);
        var specification = _spRepo.Get(group.SpecificationId);
        var typeId = group.MeasureTypeId;
        var phyRetDes = new List<string>();
        var phyRetDeduction = 0;
        var phyRetCount = 0;
        var singleRules = new List<Rule>();
        var meanRules = new List<Rule>();
        var sdRules = new List<Rule>();
        var cpkRules = new List<Rule>();
        var cvRules = new List<Rule>();
        if (!string.IsNullOrEmpty(specification.SingleRules))
            singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);

        if (!string.IsNullOrEmpty(specification.MeanRules))
            meanRules = JsonConvert.DeserializeObject<List<Rule>>(specification.MeanRules);

        if (!string.IsNullOrEmpty(specification.SdRules))
            sdRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SdRules);

        if (!string.IsNullOrEmpty(specification.CpkRules))
            cpkRules = JsonConvert.DeserializeObject<List<Rule>>(specification.CpkRules);

        if (!string.IsNullOrEmpty(specification.CvRules))
            cvRules = JsonConvert.DeserializeObject<List<Rule>>(specification.CvRules);

        var indicatorValues = _mtiRepo.All().AsNoTracking();
        var data = (JArray)JsonConvert.DeserializeObject(dto.DataInfo);
        var appearances = new List<PhysicalReportAppearance>();
        if (singleRules != null)
            foreach (var rule in singleRules)
            {
                // 获取物理指标分值管理中的对应指标Id的数据
                var indicatorValue = indicatorValues
                    .FirstOrDefault(c => c.IndicatorId == rule.Id && c.MeasureTypeId == typeId &&
                                         c.SpecificationTypeId == specification.SpecificationTypeId);
                if (indicatorValue == null) continue;

                var standard = Convert.ToDouble(rule.Standard);
                var upper = Convert.ToDouble(rule.Upper);
                var lower = Convert.ToDouble(rule.Lower);
                // 用于暂存当前指标的测量数据
                var tempDataList = new List<double>();
                var currentAppCount = 0;

                foreach (var item in data)
                    // 当前指标不存在数据时跳过
                    if (item[$"{rule.Id}"] != null)
                    {
                        // 获取当前指标的测量值
                        var value = item[$"{rule.Id}"].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            // 指标标准为0时为外观指标
                            if (standard.Equals(0))
                            {
                                var frequency = Convert.ToInt32(value);
                                currentAppCount += frequency;
                            }
                            else
                            {
                                if (rule.Name == "水分")
                                {
                                    var water = Convert.ToDouble(value);
                                    var upLimit = standard + upper;
                                    var downLimit = standard - lower;
                                    double distance = 0;
                                    if (water > upLimit)
                                        distance = water - upLimit;
                                    else if (water < downLimit) distance = downLimit - water;

                                    var subScore = distance / 0.1 * indicatorValue.Deduction;
                                    if (subScore > indicatorValue.Points && indicatorValue.Points > 0)
                                    {
                                        phyRetDes.Add(rule.Name);
                                        phyRetDeduction += indicatorValue.Points;
                                    }
                                    else
                                    {
                                        phyRetDeduction += Convert.ToInt32(subScore);
                                    }
                                }
                                else
                                {
                                    // 将物理指标测量数据存入暂存list
                                    tempDataList.Add(Convert.ToDouble(value));
                                }
                            }
                        }
                    }

                if (standard.Equals(0))
                    if (currentAppCount > 0)
                    {
                        var appDeduction = 1;
                        if (indicatorValue != null) appDeduction = indicatorValue.Deduction;

                        if (indicatorValue.UnQualifiedOperator == UnQualifiedOperator.MoreThan)
                        {
                            if (currentAppCount > indicatorValue.UnQualifiedCount) phyRetCount++;
                        }
                        else if (indicatorValue.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual)
                        {
                            if (currentAppCount >= indicatorValue.UnQualifiedCount) phyRetCount++;
                        }

                        var subScore = currentAppCount * appDeduction;
                        var appearance = new PhysicalReportAppearance
                        {
                            IndicatorId = rule.Id,
                            Frequency = currentAppCount,
                            SubScore = subScore > indicatorValue.Points && indicatorValue.Points > 0
                                ? indicatorValue.Points
                                : subScore
                        };
                        appearances.Add(appearance);
                    }

                var statistic = tempDataList.toStatistic(standard, upper, lower);
                var cnt = statistic.HighCnt + statistic.LowCnt;
                var sdRule = sdRules.FirstOrDefault(c => c.Id == rule.Id);
                var meanRule = meanRules.FirstOrDefault(c => c.Id == rule.Id);
                var cpkRule = cpkRules.FirstOrDefault(c => c.Id == rule.Id);
                var cvRule = cvRules.FirstOrDefault(c => c.Id == rule.Id);

                #region 计算 sd 值扣分

                if (sdRule != null)
                {
                    var tempSdList = new List<double> { statistic.Sd };
                    var sdSta = tempSdList.toStatistic(double.Parse(sdRule.Standard), double.Parse(sdRule.Upper),
                        double.Parse(sdRule.Lower));
                    var sdDeduction = (sdSta.HighCnt + sdSta.LowCnt) * indicatorValue.SdDeduction;
                    if (sdDeduction > 0)
                    {
                        phyRetDes.Add($"{rule.Name}SD值");
                        if (sdDeduction > indicatorValue.SdPoints && indicatorValue.SdPoints > 0)
                            phyRetDeduction += indicatorValue.SdPoints;
                        else
                            phyRetDeduction += sdDeduction;
                    }
                }

                #endregion

                #region 计算 mean 值扣分

                if (meanRule != null)
                {
                    var tempMeanList = new List<double> { statistic.Mean };
                    var meanSta = tempMeanList.toStatistic(double.Parse(meanRule.Standard),
                        double.Parse(meanRule.Upper), double.Parse(meanRule.Lower));
                    var meanDeduction = (meanSta.HighCnt + meanSta.LowCnt) * indicatorValue.MeanDeduction;
                    if (meanDeduction > 0)
                    {
                        phyRetDes.Add($"{rule.Name}平均值");
                        if (meanDeduction > indicatorValue.MeanPoints && indicatorValue.MeanPoints > 0)
                            phyRetDeduction += indicatorValue.MeanPoints;
                        else
                            phyRetDeduction += meanDeduction;
                    }
                }

                #endregion

                #region 计算 cpk 值扣分

                //if (cpkRule != null)
                //{
                //    var tempCpkList = new List<double> { statistic.Cpk };
                //    var cpkSta = tempCpkList.toStatistic(double.Parse(cpkRule.Standard),
                //        double.Parse(cpkRule.Upper), double.Parse(cpkRule.Lower));
                //    var cpkDeduction = (cpkSta.HighCnt + cpkSta.LowCnt) * indicatorValue.CvDeduction;
                //    if (cpkDeduction > 0)
                //    {
                //        phyRetDes.Add($"{rule.Name}CPK值");
                //    }
                //    if (cpkDeduction > indicatorValue.CvPoints && indicatorValue.CvPoints > 0)
                //    {
                //        phyRetDeduction += indicatorValue.CvPoints;
                //    }
                //    else
                //    {
                //        phyRetDeduction += cpkDeduction;
                //    }
                //}

                #endregion

                #region 计算 cv 值扣分

                if (cvRule != null)
                {
                    var tempCvList = new List<double> { statistic.Cv };
                    var cvSta = tempCvList.toStatistic(double.Parse(cvRule.Standard),
                        double.Parse(cvRule.Upper), double.Parse(cvRule.Lower));
                    var cvDeduction = (cvSta.HighCnt + cvSta.LowCnt) * indicatorValue.CvDeduction;
                    if (cvDeduction > 0)
                    {
                        phyRetDes.Add($"{rule.Name}CV值");
                        if (cvDeduction > indicatorValue.CvPoints && indicatorValue.CvPoints > 0)
                            phyRetDeduction += indicatorValue.CvPoints;
                        else
                            phyRetDeduction += cvDeduction;
                    }
                }

                #endregion

                if (indicatorValue != null)
                {
                    var deduction = cnt * indicatorValue.Deduction;
                    if (deduction > indicatorValue.Points && indicatorValue.Points > 0)
                        phyRetDeduction += indicatorValue.Points;
                    else
                        phyRetDeduction += deduction;

                    if (indicatorValue.UnQualifiedOperator == UnQualifiedOperator.MoreThan)
                    {
                        if (cnt > indicatorValue.UnQualifiedCount)
                        {
                            phyRetCount++;
                            phyRetDes.Add(rule.Name);
                        }
                    }
                    else if (indicatorValue.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual)
                    {
                        if (cnt >= indicatorValue.UnQualifiedCount)
                        {
                            phyRetCount++;
                            phyRetDes.Add(rule.Name);
                        }
                    }
                }
            }


        report.PhyRetDeduction = phyRetDeduction;
        report.PhyRetDes = string.Join(",", phyRetDes);
        if (phyRetCount > 0)
        {
            report.PhyRet = ReportRet.不合格;
            report.FinalRet = ReportRet.不合格;
            report.Remark = "复检合格";
        }
        else
        {
            report.PhyRet = ReportRet.合格;
            report.FinalRet = ReportRet.合格;
            report.Remark = "";
        }

        if (isEdit)
            _prRepo.Update(report);
        else
            _prRepo.Add(report);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;
        var oldAppearance = _praRepo.All().Where(c => c.ReportId == report.Id).ToList();
        _praRepo.DeleteRange(oldAppearance);
        foreach (var appearance in appearances) appearance.ReportId = report.Id;

        _praRepo.AddRange(appearances);
        result = _uow.Save() >= 0;
        return result;
    }

    public ReportStatisticDto GetStatisticInfo(int id, out string failReason)
    {
        var report = _prRepo.Get(id);
        var mdService = _services.GetRequiredService<IMetricalDataService>();
        var groupStatistic = mdService.GetStatistic(report.GroupId, out var fail);
        var group = _gRepo.All().Include(c => c.Specification).Include(c => c.Turn)
            .Include(c => c.MachineModel).FirstOrDefault(c => c.Id == report.GroupId);
        var appearances = _praRepo.All().Include(c => c.Indicator).Where(c => c.ReportId == id)
            .Select(c => new
            {
                c.Indicator.Name,
                c.SubScore
            }).ToList();
        var phyRetDes = !string.IsNullOrEmpty(report.PhyRetDes)
            ? report.PhyRetDes.Trim().Split(",").ToList()
            : null;
        var dto = new ReportStatisticDto
        {
            SpecificationName = group.Specification.Name,
            TurnName = group.Turn.Name,
            MachineModelName = group.MachineModel.Name,
            StatisticColumns = groupStatistic.StatisticColumns,
            StatisticData = groupStatistic.DataInfo,
            OriginColumns = groupStatistic.OriginColumns,
            OriginData = groupStatistic.OriginDataInfo,
            Appearances = appearances.Count > 0
                ? appearances.Select(c => c.Name).ToList()
                : null,
            Points = appearances.Sum(c => c.SubScore) + report.PhyRetDeduction,
            PhyRetDes = phyRetDes,
            FinalRet = report.FinalRet.ToString()
        };

        failReason = fail;

        return dto;
    }

    public IEnumerable<ReportTableDto> GetTable(QueryInfoDto dto, out int total)
    {
        var data = _prRepo.All().AsNoTracking();
        var roleId = _accessor.HttpContext.getUserRoleId();
        var canSeeOtherData = _accessor.HttpContext.getCanSeeOtherData();
        if (roleId != _settings.AdminTypeId)
            if (!canSeeOtherData)
            {
                var userId = _accessor.HttpContext.getUserId();
                data = data.Where(c => c.Group.UserId == userId || c.Group.UserId == 0);
            }

        if (!string.IsNullOrEmpty(dto.BeginDate) && !string.IsNullOrEmpty(dto.EndDate))
        {
            var begin = Convert.ToDateTime(dto.BeginDate);
            var end = Convert.ToDateTime(dto.EndDate);
            data = data.Where(c => c.Group.BeginTime.Date >= begin &&
                                   c.Group.EndTime.Date <= end);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = Convert.ToInt32(dto.SpecificationId);
            data = data.Where(c =>
                c.Group.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var typeId = int.Parse(dto.SpecificationTypeId);
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
            data = data.Where(c =>
                c.Group.MachineModelId == machineModelId);
        }

        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c =>
                c.Group.Specification.Name.Contains(dto.Query));

        total = data.Count();

        var list = data.OrderByDescending(c => c.Group.BeginTime).Include(c => c.Group)
            .ThenInclude(c => c.Specification)
            .Include(c => c.Group).ThenInclude(c => c.Turn)
            .Include(c => c.Group).ThenInclude(c => c.MachineModel)
            .Skip(dto.Skip()).Take(dto.PageSize).Select(c =>
                new ReportTableDto
                {
                    Id = c.Id,
                    BeginTime =
                        c.Group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProductDate = c.Group.ProductionTime == null
                        ? ""
                        : Convert.ToDateTime(c.Group
                            .ProductionTime).ToString("yyyy-MM-dd"),
                    SpecificationName = c.Group.Specification.Name,
                    TurnName = c.Group.Turn.Name,
                    MachineModelName = c.Group.MachineModel.Name,
                    FinalRet = Convert.ToInt32(c.FinalRet),
                    PhyScore = 100 - c.PhyRetDeduction,
                    UserName = c.Group.UserName
                }).ToList();

        return list;
    }

    public MemoryStream Download(int id)
    {
        throw new NotImplementedException();
    }
}