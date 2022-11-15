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
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services.Reports.Impl;

public class FactoryReportService : IFactoryReportService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<FactoryReport> _frRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<MeasureTypeIndicators> _mtiRepo;
    private readonly IRepository<Role> _rRepo;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;
    private readonly IRepository<SpecificationTypeRule> _sptRepo;
    private readonly IRepository<SpecificationType> _stRepo;
    private readonly IUnitOfWork _uow;
    private readonly IUserService _userService;
    private readonly IRepository<WaterRecord> _wrRepo;

    public FactoryReportService(IRepository<FactoryReport> frRepo,
        IRepository<Group> gRepo, IRepository<Data> dRepo,
        IRepository<Core.Entities.Specification> spRepo, IRepository<Core.Entities.Indicator> iRepo,
        IOptionsSnapshot<Settings> settings, IHttpContextAccessor accessor,
        IRepository<SpecificationTypeRule> sptRepo,
        IRepository<SpecificationType> stRepo,
        IUserService userService, IUnitOfWork uow,
        IRepository<Role> rRepo, IRepository<WaterRecord> wrRepo,
        IRepository<MeasureTypeIndicators> mtiRepo)
    {
        _frRepo = frRepo;
        _gRepo = gRepo;
        _dRepo = dRepo;
        _spRepo = spRepo;
        _iRepo = iRepo;
        _accessor = accessor;
        _sptRepo = sptRepo;
        _stRepo = stRepo;
        _userService = userService;
        _uow = uow;
        _rRepo = rRepo;
        _wrRepo = wrRepo;
        _mtiRepo = mtiRepo;
        _settings = settings.Value;
    }

    public IEnumerable<ReportTableDto> GetTable(QueryInfoDto dto, out int total)
    {
        var data = _frRepo.All().AsNoTracking();
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
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var typeId = Convert.ToInt32(dto.SpecificationTypeId);
            data = data.Where(c => c.Specification.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.BeginDate) && !string.IsNullOrEmpty(dto.EndDate))
        {
            var begin = Convert.ToDateTime(dto.BeginDate);
            var end = Convert.ToDateTime(dto.EndDate);
            data = data.Where(c => c.Group.BeginTime.Date >= begin &&
                                   c.Group.EndTime.Date <= end);
        }

        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c => c.Specification.Name.Contains(dto.Query) ||
                                   c.ManufacturerPlace.Contains(dto.Query) ||
                                   c.TestMethod.Contains(dto.Query) ||
                                   c.User.NickName.Contains(dto.Query));

        total = data.Count();
        var result = data.OrderByDescending(c => c.TestDate).Include(c => c.Specification)
            .Include(c => c.User).Skip(dto.Skip()).Take(dto.PageSize).Select(c => new
                ReportTableDto
                {
                    Id = c.Id, BeginTime = c.Group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProductDate = c.Group.ProductionTime != null
                        ? Convert.ToDateTime(c.Group.ProductionTime).ToString
                            ("yyyy-MM-dd")
                        : "",
                    SpecificationId = c.Group.SpecificationId,
                    SpecificationName = c.Group.Specification.Name,
                    FinalRet = (int)c.ReportRet, UserName = c.User.NickName,
                    TypeId = c.Group.Specification.SpecificationTypeId
                }).ToList();
        return result;
    }

    public bool Add(MetricalDataEditDataDto dto)
    {
        var report = new FactoryReport
        {
            GroupId = dto.GroupId
        };
        var isEdit = false;
        if (_frRepo.All().Any(c => c.GroupId == dto.GroupId))
        {
            isEdit = true;
            report = _frRepo.All().FirstOrDefault(c => c.GroupId == dto.GroupId);
        }

        if (report == null) return false;

        var group = _gRepo.Get(dto.GroupId);
        report.SpecificationId = group.SpecificationId;
        report.TestDate = group.BeginTime;
        report.UserId = _accessor.HttpContext.getUserId();
        report.ReportRet = ReportRet.未完成;
        report.ReportType = ReportType.Self;
        report.OrderNo = group.OrderNo;
        report.AuditUser = _userService.GetUserName(_settings.FactoryAuditUser);

        if (isEdit)
            _frRepo.Update(report);
        else
            _frRepo.Add(report);

        var result = isEdit ? _uow.Save() >= 0 : _uow.Save() > 0;
        return result;
    }

    public FactoryReportEditDto GetInfo(int id)
    {
        var dto = new FactoryReportEditDto();
        var data = _frRepo.All().Include(c => c.Group).Include(c => c.Specification)
            .FirstOrDefault(c => c.Id == id);
        if (data == null) return dto;

        var specificationType = _stRepo.Get(data.Specification.SpecificationTypeId);

        dto.Id = data.Id;
        dto.SpecificationName = data.Specification.Name;
        dto.BeginTime = data.Group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
        dto.ProductDate = data.Group.ProductionTime != null
            ? Convert.ToDateTime(data.Group.ProductionTime)
                .ToString("yyyy-MM-dd")
            : "";
        dto.GroupIds = string.IsNullOrEmpty(data.GroupIds)
            ? new List<int>()
            : JsonConvert.DeserializeObject<List<int>>(data.GroupIds);
        dto.ManufacturerPlace = data.ManufacturerPlace;
        dto.TestMethod = data.TestMethod;
        dto.Result = data.Result;
        dto.OrderNo = string.IsNullOrEmpty(data.OrderNo) ? specificationType.Remark : data.OrderNo;
        dto.FinalRet = data.ReportRet == ReportRet.未完成 || (data.ReportRet == ReportRet.合格 ? true : false);
        dto.ReportType = (int)data.ReportType;
        dto.ChemicalDataId = data.ChemicalDataId;
        dto.Count = data.Count;
        dto.AuditUser = data.AuditUser;
        dto.Id = data.Id;
        dto.SpecificationName = data.Specification.Name;
        dto.BeginTime = data.Group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
        dto.ProductDate = data.Group.ProductionTime != null
            ? Convert.ToDateTime(data.Group.ProductionTime)
                .ToString("yyyy-MM-dd")
            : "";
        dto.ManufacturerPlace = data.ManufacturerPlace;
        dto.TestMethod = data.TestMethod;
        dto.Result = data.Result;
        dto.OrderNo = string.IsNullOrEmpty(data.OrderNo) ? specificationType.Remark : data.OrderNo;
        dto.FinalRet = data.ReportRet == ReportRet.未完成 || (data.ReportRet == ReportRet.合格 ? true : false);
        dto.ReportType = (int)data.ReportType;
        dto.ChemicalDataId = data.ChemicalDataId;
        dto.CountInBox = data.CountInBox;
        dto.Count = data.Count;
        dto.AuditUser = data.AuditUser;

        return dto;
    }

    public bool Edit(FactoryReportEditDto dto)
    {
        var report = _frRepo.Get(dto.Id);
        report.ManufacturerPlace = dto.ManufacturerPlace;
        report.GroupIds = JsonConvert.SerializeObject(dto.GroupIds);
        report.TestMethod = dto.TestMethod;
        report.Result = dto.Result;
        report.ReportRet = dto.FinalRet ? ReportRet.合格 : ReportRet.不合格;
        report.OrderNo = dto.OrderNo;
        report.ReportType = (ReportType)dto.ReportType;
        report.ChemicalDataId = dto.ChemicalDataId;
        report.Count = dto.Count;
        report.CountInBox = dto.CountInBox;
        report.AuditUser = _userService.GetUserName(_settings.FactoryAuditUser);

        _frRepo.Update(report);
        return _uow.Save() >= 0;
    }

    public MemoryStream Download(int id)
    {
        throw new NotImplementedException();
    }

    public FactoryReportDownloadDto GetReportInfo(int id)
    {
        var data = getDownloadInfo(id);
        return data;
    }

    private FactoryReportDownloadDto getDownloadInfo(int id)
    {
        var dto = new FactoryReportDownloadDto();
        var report = _frRepo.All().Include(c => c.Group)
            .ThenInclude(c => c.Specification).ThenInclude(c => c.SpecificationType).Include(c => c.User)
            .FirstOrDefault(c => c.Id == id);

        if (report == null) return dto;

        var specificationType = _stRepo.Get(report.Group.Specification.SpecificationTypeId);
        var deliverDate = report.Group.DeliverTime == null
            ? DateTime.Now
            : Convert.ToDateTime(report.Group.DeliverTime);
        dto = new FactoryReportDownloadDto
        {
            SpecificationTypeName = report.Group.Specification.SpecificationType.Name,
            SpecificationName = report.Group.Specification.Name,
            SpecificationOrderNo = report.Group.Specification.OrderNo,
            InitialDate = report.Group.BeginTime.ToString("yyyy年MM月dd日"),
            TestDate = report.Group.BeginTime.ToString("yyyy年MM月dd日"),
            Factory = report.ManufacturerPlace,
            Result = report.Result,
            Checker = report.User.NickName,
            OrderNo = report.OrderNo,
            ReportType = (int)report.ReportType,
            MeasureMethod = report.TestMethod,
            Count = report.Count,
            AuditUser = _userService.GetUserName(_settings.FactoryAuditUser),
            ProductDate = report.Group.ProductionTime == null
                ? ""
                : Convert.ToDateTime(report.Group.ProductionTime).ToString("yyyy年MM月dd日")
        };

        var indicators = _iRepo.All().AsNoTracking();
        // 获取牌号
        var specification = _spRepo.Get(report.Group.SpecificationId);
        // 获取牌号单支规则
        var singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);

        #region 获取化学检验数据

        var chemicalDataItems = new List<FactoryReportDownloadDto.IndicatorDataItem>();
        if (report.ChemicalDataId > 0)
        {
            var chemical = _sptRepo.All()
                .FirstOrDefault(c => c.SpecificationTypeId == report.Group.Specification.SpecificationTypeId);
            if (chemical == null)
                return dto;
            var chemicalRules = JsonConvert.DeserializeObject<List<Rule>>(chemical.Rules);
            if (chemicalRules == null)
                return dto;
            var chemicalData = _dRepo.All().Include(c => c.Group)
                .FirstOrDefault(c => c.GroupId == report.ChemicalDataId);
            if (chemicalData != null)
            {
                dto.ChemicalDate = chemicalData.Group.BeginTime.ToString("yyyy年MM月dd日");
                var dataItems = JsonConvert.DeserializeObject<JObject>(chemicalData.Data);
                if (dataItems == null)
                    return dto;
                foreach (var rule in chemicalRules)
                {
                    var value = dataItems[$"{rule.Id}"];
                    if (value == null)
                        continue;
                    var ruleName = rule.Name;
                    var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                    if (indicator == null)
                        continue;
                    if (string.IsNullOrEmpty(ruleName)) ruleName = indicator.Name;

                    var result = value.ToString();
                    if (string.IsNullOrEmpty(result))
                        continue;
                    var chemicalDataItem = new FactoryReportDownloadDto.IndicatorDataItem
                    {
                        Name = ruleName,
                        Standard = Helper.Helper.getStandard(rule.Standard, rule.Upper, rule.Lower),
                        Unit = indicator.Unit,
                        Value = result == "0" ? "未检出" : result
                    };
                    chemicalDataItems.Add(chemicalDataItem);
                }
            }
        }

        #endregion

        // 滤棒发货数据
        if (specificationType.Id == _settings.FilterTypeId)
        {
            var items = new List<FactoryReportDownloadDto.FilterDataItem>();
            // 获取所有对应的测量数据
            var data = _dRepo.All().Where(c => c.GroupId == report.GroupId).ToList();

            if (singleRules == null)
                return dto;

            var indicatorRules = _mtiRepo.All().Where(c =>
                    c.MeasureTypeId == _settings.ProductionTypeId && c.SpecificationTypeId == specificationType.Id)
                .ToList();
            // 设置需要获取指标的 id
            var filterKeys = new List<int>
            {
                _settings.Weight,
                _settings.Circle,
                _settings.Oval,
                _settings.Length,
                _settings.Resistance,
                _settings.Hardness
            };
            // 获取测量值
            var dataList = getMetricalData(data, filterKeys);
            var weightList = dataList[_settings.Weight];
            var circleList = dataList[_settings.Circle];
            var ovalList = dataList[_settings.Oval];
            var lengthList = dataList[_settings.Length];
            var resistanceList = dataList[_settings.Resistance];
            var hardnessList = dataList[_settings.Hardness];
            var resistanceCv = "";

            #region 获取统计值，并将没有数据的指标剔除

            var weightRule = singleRules.FirstOrDefault(c => c.Id == _settings.Weight);
            if (weightRule != null)
            {
                var item = getFilterDataItem(weightList, weightRule.Standard, weightRule.Upper,
                    weightRule.Lower, _settings.Weight);
                if (item != null)
                {
                    var indicatorRule = indicatorRules.FirstOrDefault(c => c.IndicatorId == _settings.Weight);
                    if (indicatorRule != null)
                    {
                        var deduction = item.UnQualified * indicatorRule.Deduction;
                        item.Judgment =
                            (indicatorRule.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual
                                ? deduction < indicatorRule.Points
                                : deduction <= indicatorRule.Points)
                                ? "合格"
                                : "不合格";
                        items.Add(item);
                    }
                }
            }

            var circleRule = singleRules.FirstOrDefault(c => c.Id == _settings.Circle);
            if (circleRule != null)
            {
                var item = getFilterDataItem(circleList, circleRule.Standard, circleRule.Upper,
                    circleRule.Lower, _settings.Circle);
                if (item != null)
                {
                    var circle = indicatorRules.FirstOrDefault
                        (c => c.IndicatorId == _settings.Circle);
                    if (circle != null)
                    {
                        var deduction = item.UnQualified * circle.Deduction;
                        item.Judgment =
                            (circle.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual
                                ? deduction < circle.Points
                                : deduction <= circle.Points)
                                ? "合格"
                                : "不合格";
                        items.Add(item);
                    }
                }
            }

            var oval = singleRules.FirstOrDefault(c => c.Id == _settings.Oval);
            if (oval != null)
            {
                var item = getFilterDataItem(ovalList, oval.Standard, oval.Upper, oval.Lower, _settings.Oval);
                if (item != null)
                {
                    var ovalRule = indicatorRules.FirstOrDefault(c => c.IndicatorId == _settings.Oval);
                    if (ovalRule != null)
                    {
                        var deduction = item.UnQualified * ovalRule.Deduction;
                        item.Judgment =
                            (ovalRule.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual
                                ? deduction < ovalRule.Points
                                : deduction <= ovalRule.Points)
                                ? "合格"
                                : "不合格";
                        items.Add(item);
                    }
                }
            }

            var length = singleRules.FirstOrDefault(c => c.Id == _settings.Length);
            if (length != null)
            {
                var item = getFilterDataItem(lengthList, length.Standard, length.Upper, length.Lower,
                    _settings.Length);
                if (item != null)
                {
                    var lengthRule = indicatorRules.FirstOrDefault(c => c.IndicatorId == _settings.Length);
                    if (lengthRule != null)
                    {
                        var deduction = lengthRule.Deduction * item.UnQualified;
                        item.Judgment =
                            (lengthRule.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual
                                ? deduction < lengthRule.Points
                                : deduction <= lengthRule.Points)
                                ? "合格"
                                : "不合格";
                        items.Add(item);
                    }
                }
            }

            var resistance = singleRules.FirstOrDefault(c => c.Id == _settings.Resistance);
            if (resistance != null)
            {
                var item = getFilterDataItem(resistanceList, resistance.Standard, resistance.Upper,
                    resistance.Lower, _settings.Resistance);
                if (item != null)
                {
                    var resistanceRule = indicatorRules.FirstOrDefault(c => c.IndicatorId == _settings.Resistance);
                    if (resistanceRule != null)
                    {
                        var deduction = resistanceRule.Deduction * item.UnQualified;
                        item.Judgment =
                            (resistanceRule.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual
                                ? deduction < resistanceRule.Points
                                : deduction <= resistanceRule.Points)
                                ? "合格"
                                : "不合格";
                        resistanceCv = item.Cv;
                        items.Add(item);
                    }
                }
            }

            var hardness = singleRules.FirstOrDefault(c => c.Id == _settings.Hardness);
            if (hardness != null)
            {
                var item = getFilterDataItem(hardnessList, hardness.Standard, hardness.Upper,
                    hardness.Lower, _settings.Hardness);
                if (item != null)
                {
                    var hardnessRule = indicatorRules.FirstOrDefault(c => c.IndicatorId == _settings.Hardness);
                    if (hardnessRule != null)
                    {
                        var deduction = item.UnQualified * hardnessRule.Deduction;
                        item.Judgment =
                            (hardnessRule.UnQualifiedOperator == UnQualifiedOperator.MoreAndEqual
                                ? deduction < hardnessRule.Points
                                : deduction <= hardnessRule.Points)
                                ? "合格"
                                : "不合格";
                        items.Add(item);
                    }
                }
            }

            #endregion

            var otherFilterDataItems = new List<FactoryReportDownloadDto.FilterDataItem>();
            // 获取水分数据
            var waterRecords = _wrRepo.All().Where(c => c.GroupId == report.GroupId).ToList();
            if (waterRecords.Count > 0)
            {
                var water = indicators.FirstOrDefault(c => c.Id == _settings.WaterIndicatorId);
                var waterRule = singleRules.FirstOrDefault(c => c.Id == _settings.WaterIndicatorId);
                var waterItem = new FactoryReportDownloadDto.FilterDataItem
                {
                    Name = water.Name,
                    Unit = water.Unit,
                    Standard = waterRule == null
                        ? water.Standard
                        : Helper.Helper.getStandard(waterRule.Standard, waterRule.Upper, waterRule.Lower),
                    Value = waterRecords.Average(c => c.Water).ToString("F2")
                };
                otherFilterDataItems.Add(waterItem);
            }

            var resistanceCvItem = new FactoryReportDownloadDto.FilterDataItem
            {
                Name = "压降变异系数",
                Unit = "%",
                Standard = "",
                Value = resistanceCv
            };

            otherFilterDataItems.Add(resistanceCvItem);

            var countInBox = report.CountInBox;
            var countInBoxItem = new FactoryReportDownloadDto.FilterDataItem
            {
                Name = "数量",
                Unit = "支/盒",
                Standard = "4490 - 4520",
                Value = countInBox.ToString(),
                Judgment = countInBox > 4520 || countInBox < 4490 ? "不合格" : "合格"
            };

            otherFilterDataItems.Add(countInBoxItem);

            dto.OtherFilterDataItems = otherFilterDataItems;

            // 外观项
            var appDataItem = new FactoryReportDownloadDto.IndicatorDataItem
            {
                Name = "外观",
                Unit = null,
                Standard = "——",
                Value = "符合"
            };
            var appDataItemValues = new List<int>();
            var appItems = new List<FactoryReportDownloadDto.IndicatorDataItem>();
            foreach (var rule in singleRules)
            {
                // 只获取外观指标
                if (rule.Standard != "0")
                    continue;
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                if (indicator == null)
                    continue;
                var name = rule.Name == "" ? indicator.Name : rule.Name;
                var item = new FactoryReportDownloadDto.IndicatorDataItem
                {
                    Name = name,
                    Unit = indicator.Unit == "" ? null : indicator.Unit,
                    Standard = indicator.Standard
                };

                // 指标值缓存
                var valueList = new List<string>();
                foreach (var d in data)
                {
                    var temp = JsonConvert.DeserializeObject<JObject>(d.Data);
                    var value = temp[rule.Id.ToString()];
                    if (value == null) continue;
                    var result = value.ToString();
                    // 如果外观指标填写了数据，则表示存在外观缺陷，将数据添加到指标值缓存中
                    if (!string.IsNullOrEmpty(result))
                        valueList.Add(result);
                }

                item.Type = 0;
                // 如果存在指标值则表示该外观指标不符合标准
                if (valueList.Count > 0)
                {
                    item.Value = "不符合";
                    // 将不符合要求的外观项缓存中
                    appDataItemValues.Add(1);
                }
                else
                {
                    item.Value = "符合";
                    appDataItemValues.Add(0);
                }

                if (valueList.Count > 0)
                    appItems.Add(item);
            }

            // 如果外观项缓存存在外观指标则表示外观项不符合标准
            if (appDataItemValues.Any(c => c == 1)) appDataItem.Value = "不符合";

            dto.FilterDataItems = items;
            appItems.Add(appDataItem);
            dto.IndicatorDataItems = appItems;
        }
        // 其他类型发货数据
        else
        {
            var items = new List<FactoryReportDownloadDto.IndicatorDataItem>();
            var data = _dRepo.All().Where(c => c.GroupId == report.GroupId).ToList();
            FactoryReportDownloadDto.IndicatorDataItem orderDataItem = null;
            if (report.ReportType == ReportType.Cooperation)
                if (!string.IsNullOrEmpty(specification.OrderNo))
                    orderDataItem = new FactoryReportDownloadDto.IndicatorDataItem
                    {
                        Name = "材料编号",
                        Unit = null,
                        Standard = specification.OrderNo,
                        Value = "符合"
                    };

            var appDataItem = new FactoryReportDownloadDto.IndicatorDataItem
            {
                Name = "外观",
                Unit = null,
                Standard = "——",
                Value = "符合"
            };
            var appDataItemValue = new List<int>();

            foreach (var rule in singleRules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                var ruleName = rule.Name == "" ? indicator.Name : rule.Name;
                var item = new FactoryReportDownloadDto.IndicatorDataItem
                {
                    Name = ruleName,
                    Unit = indicator.Unit == "" ? null : indicator.Unit
                };
                var standard = "";
                standard = rule.Standard == "0"
                    ? indicator.Standard
                    : Helper.Helper.getStandard(rule.Standard, rule.Upper, rule.Lower);

                item.Standard = standard;
                var valueList = new List<string>();
                foreach (var d in data)
                {
                    var measure = (JObject)JsonConvert.DeserializeObject(d.Data);
                    var value = measure[$"{rule.Id}"];
                    if (value != null)
                    {
                        var result = value.ToString();
                        if (result != "")
                            valueList.Add(value.ToString());
                    }
                }

                if (rule.Standard == "0")
                {
                    item.Type = 0;
                    if (valueList.Count == 0)
                    {
                        item.Value = "符合";
                        if (string.IsNullOrEmpty(standard))
                            appDataItemValue.Add(1);
                    }
                    else
                    {
                        item.Value = "不符合";
                        if (string.IsNullOrEmpty(standard))
                            appDataItemValue.Add(0);
                    }
                }
                else
                {
                    item.Type = 1;
                    item.Values = valueList;
                }

                if ((rule.Standard != "0" && valueList.Count > 0) ||
                    (rule.Standard == "0" && !string.IsNullOrEmpty(standard)))
                    items.Add(item);
            }

            if (appDataItemValue.Any(c => c == 0)) appDataItem.Value = "不符合";

            items.Add(appDataItem);
            dto.IndicatorDataItems = items.OrderByDescending(c => c.Type).ToList();
            if (orderDataItem != null)
                dto.IndicatorDataItems.Insert(0, orderDataItem);
            dto.ChemicalDataItems = chemicalDataItems;
            dto.MeasureMethod = report.TestMethod;
        }

        // 获取滤棒类型数据续表
        if (!string.IsNullOrEmpty(report.GroupIds))
        {
            var filterGroupsDataItems = new List<FactoryReportDownloadDto.FilterGroupDataItem>();
            var groupIds = JsonConvert.DeserializeObject<List<int>>(report.GroupIds);
            dto.FilterGroups = groupIds.Count;
            var dataList = _dRepo.All().Where(c => groupIds.Contains(c.GroupId)).ToList();
            var groupList = dataList.GroupBy(c => c.GroupId).ToList();
            var getKeys = new List<int>
            {
                _settings.Weight,
                _settings.Circle,
                _settings.Oval,
                _settings.Length,
                _settings.Resistance,
                _settings.Hardness,
                _settings.WaterIndicatorId,
                _settings.Burst,
                _settings.GlueHole,
                _settings.PeculiarSmell,
                _settings.InnerBondLine
            };

            var weightItem = new FactoryReportDownloadDto.FilterGroupDataItem
            {
                Name = "重量",
                Standard = ""
            };

            var circleItem = new FactoryReportDownloadDto.FilterGroupDataItem
            {
                Name = "圆周",
                Standard = ""
            };

            var ovalItem = new FactoryReportDownloadDto.FilterGroupDataItem
            {
                Name = "圆度",
                Standard = ""
            };

            var lengthItem = new FactoryReportDownloadDto.FilterGroupDataItem
            {
                Name = "长度",
                Standard = ""
            };

            var resistanceItem = new FactoryReportDownloadDto.FilterGroupDataItem
            {
                Name = "吸阻",
                Standard = ""
            };

            var hardnessItem = new FactoryReportDownloadDto.FilterGroupDataItem
            {
                Name = "硬度",
                Standard = ""
            };

            var burstItems = new List<int>();
            var glueHoleItems = new List<int>();
            var peculiarSmellItems = new List<int>();
            var innerBondLineItems = new List<int>();

            var waterMeanLs = new List<double>();
            foreach (var data in groupList)
            {
                var origin = getMetricalData(data, getKeys);
                var weightLs = origin[_settings.Weight];
                var circleLs = origin[_settings.Circle];
                var ovalLs = origin[_settings.Oval];
                var lengthLs = origin[_settings.Length];
                var resistanceLs = origin[_settings.Resistance];
                var hardnessLs = origin[_settings.Hardness];
                var waterLs = origin[_settings.WaterIndicatorId];
                var burstLs = origin[_settings.Burst];
                var glueHoleLs = origin[_settings.GlueHole];
                var peculiarSmellLs = origin[_settings.PeculiarSmell];
                var innerBondLineLs = origin[_settings.InnerBondLine];

                if (waterLs.Count > 0) waterMeanLs.Add(waterLs.Average());

                if (weightLs.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Weight);
                    var result = getFilterDataItem(weightLs, rule.Standard, rule.Upper, rule.Lower,
                        _settings.Weight);
                    if (result != null)
                    {
                        weightItem.MaxList.Add(result.Max);
                        weightItem.MinList.Add(result.Min);
                        weightItem.MeanList.Add(result.Mean);
                        weightItem.SdList.Add(result.Sd);
                        weightItem.UnqualifiedList.Add(result.UnQualified);
                    }
                }

                if (circleLs.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Circle);
                    var result = getFilterDataItem(circleLs, rule.Standard, rule.Upper, rule.Lower,
                        _settings.Circle);
                    if (result != null)
                    {
                        circleItem.MaxList.Add(result.Max);
                        circleItem.MinList.Add(result.Min);
                        circleItem.MeanList.Add(result.Mean);
                        circleItem.SdList.Add(result.Sd);
                        circleItem.UnqualifiedList.Add(result.UnQualified);
                    }
                }

                if (ovalLs.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Oval);
                    var result = getFilterDataItem(ovalLs, rule.Standard, rule.Upper, rule.Lower,
                        _settings.Oval);
                    if (result != null)
                    {
                        ovalItem.MaxList.Add(result.Max);
                        ovalItem.MinList.Add(result.Min);
                        ovalItem.MeanList.Add(result.Mean);
                        ovalItem.SdList.Add(result.Sd);
                        ovalItem.UnqualifiedList.Add(result.UnQualified);
                    }
                }

                if (lengthLs.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Length);
                    var result = getFilterDataItem(lengthLs, rule.Standard, rule.Upper, rule.Lower,
                        _settings.Length);
                    if (result != null)
                    {
                        lengthItem.MaxList.Add(result.Max);
                        lengthItem.MinList.Add(result.Min);
                        lengthItem.MeanList.Add(result.Mean);
                        lengthItem.SdList.Add(result.Sd);
                        lengthItem.UnqualifiedList.Add(result.UnQualified);
                    }
                }

                if (resistanceLs.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Resistance);
                    var result = getFilterDataItem(resistanceLs, rule.Standard, rule.Upper, rule.Lower,
                        _settings.Resistance);
                    if (result != null)
                    {
                        resistanceItem.MaxList.Add(result.Max);
                        resistanceItem.MinList.Add(result.Min);
                        resistanceItem.MeanList.Add(result.Mean);
                        resistanceItem.SdList.Add(result.Sd);
                        resistanceItem.UnqualifiedList.Add(result.UnQualified);
                    }
                }

                if (hardnessLs.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Hardness);
                    var result = getFilterDataItem(hardnessLs, rule.Standard, rule.Upper, rule.Lower,
                        _settings.Hardness);
                    if (result != null)
                    {
                        hardnessItem.MaxList.Add(result.Max);
                        hardnessItem.MinList.Add(result.Min);
                        hardnessItem.MeanList.Add(result.Mean);
                        hardnessItem.SdList.Add(result.Sd);
                        hardnessItem.UnqualifiedList.Add(result.UnQualified);
                    }
                }
                
                // 爆口
                if (burstLs.Count > 0)
                {
                    burstItems.Add(Convert.ToInt32(burstLs.Sum()));
                }
                else
                {
                    burstItems.Add(0);
                }

                // 胶孔
                if (glueHoleLs.Count > 0)
                {
                    glueHoleItems.Add(Convert.ToInt32(glueHoleLs.Sum()));
                }
                else
                {
                    glueHoleItems.Add(0);
                }

                // 异味
                if (peculiarSmellLs.Count > 0)
                {
                    peculiarSmellItems.Add(Convert.ToInt32(peculiarSmellLs.Sum()));
                }
                else
                {
                    peculiarSmellItems.Add(0);
                }

                // 内粘接线
                if (innerBondLineLs.Count > 0)
                {
                    innerBondLineItems.Add(Convert.ToInt32(innerBondLineLs.Sum()));
                }
                else
                {
                    innerBondLineItems.Add(0);
                }
            }

            filterGroupsDataItems.Add(weightItem);
            filterGroupsDataItems.Add(circleItem);
            filterGroupsDataItems.Add(ovalItem);
            filterGroupsDataItems.Add(lengthItem);
            filterGroupsDataItems.Add(resistanceItem);
            filterGroupsDataItems.Add(hardnessItem);
            dto.FilterGroupDataItems = filterGroupsDataItems;
            dto.FilterBurstItems = burstItems;
            dto.FilterGlueHoleItems = glueHoleItems;
            dto.FilterPeculiarSmellItems = peculiarSmellItems;
            dto.FilterInnerBondLineItems = innerBondLineItems;
            if (waterMeanLs.Count > 0)
                dto.FilterGroupWater = waterMeanLs.Average().ToString("F2");
        }


        return dto;
    }

    /// <summary>
    ///     获取指标统计信息
    /// </summary>
    /// <param name="list">指标数据</param>
    /// <param name="standard">标准值</param>
    /// <param name="upper">上限值</param>
    /// <param name="lower">下限值</param>
    /// <param name="key">指标Id</param>
    /// <returns></returns>
    private FactoryReportDownloadDto.FilterDataItem getFilterDataItem(List<double> list, string standard,
        string upper, string lower, int key)
    {
        var result = new FactoryReportDownloadDto.FilterDataItem();
        if (list == null || list.Count == 0 || !list.Any(c => c > 0)) return null;

        if (string.IsNullOrEmpty(standard))
            return null;
        var sta = list.toStatistic(standard, upper, lower);
        var item = _iRepo.All().FirstOrDefault(c => c.Id == key);
        result.Name = item?.Name;
        result.Standard = Helper.Helper.getStandard(standard, upper, lower);
        result.Unit = item?.Unit;
        result.Max = sta.Max.ToString("F3");
        result.Min = sta.Min.ToString("F3");
        result.Mean = sta.Mean.ToString("F3");
        result.Sd = sta.Sd.ToString("F3");
        result.Cv = sta.Cv.ToString("F3");
        result.UnQualified = sta.HighCnt + sta.LowCnt;
        result.Judgment = result.UnQualified > 3 ? "不合格" : "合格";
        return result;
    }

    /// <summary>
    ///     获取数据表中指标和数据的集合
    /// </summary>
    /// <param name="originDataList">原始数据表</param>
    /// <param name="keys">需要获取的指标Id</param>
    /// <returns></returns>
    private Dictionary<int, List<double>> getMetricalData(IEnumerable<Data> originDataList, List<int> keys)
    {
        var result = new Dictionary<int, List<double>>();
        foreach (var key in keys) result[key] = new List<double>();
        foreach (var dataItem in originDataList)
        {
            var data = JsonConvert.DeserializeObject<JObject>(dataItem.Data);
            if (data == null)
                continue;
            foreach (var key in keys)
            {
                var value = data[key.ToString()]?.ToString();
                if (string.IsNullOrEmpty(value))
                    continue;
                var item = Convert.ToDouble(value);
                result[key].Add(item);
            }
        }

        return result;
    }
}