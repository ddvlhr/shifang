﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Dtos.MetricalData;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Infrastructure.Helper;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Infrastructure.Services.MetricalData.Impl;

public class MetricalDataService : IMetricalDataService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IRepository<CraftIndicatorRule> _cirRepo;
    private readonly IRepository<CraftReport> _crRepo;
    private readonly IRepository<TestReport> _trRepo;
    private readonly ITestReportService _trService;
    private readonly ICraftReportService _crService;
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<DataRecord> _drRepo;
    private readonly IRepository<FactoryReport> _frRepo;
    private readonly IFactoryReportService _frService;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<GroupRecord> _grRepo;
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<InspectionReport> _irRepo;
    private readonly IInspectionReportService _irService;
    private readonly IRepository<MachineModel> _mmRepo;
    private readonly IRepository<MaterialReport> _mrRepo;
    private readonly IMaterialReportService _mrService;
    private readonly IRepository<ProductReport> _prRepo;
    private readonly IPhysicalReportService _prService;
    private readonly IProductReportService _prsService;
    private readonly IRepository<PhysicalReport> _psrRepo;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;
    private readonly IRepository<SpecificationTypeRule> _sptRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<WaterRecord> _wrRepo;

    public MetricalDataService(IRepository<Group> gRepo, IRepository<Core.Entities.Specification> spRepo,
        IRepository<Data> dRepo, IRepository<Core.Entities.Indicator> iRepo, IPhysicalReportService prService,
        IInspectionReportService irService,
        IProductReportService prsService, IUnitOfWork uow, IRepository<InspectionReport> irRepo,
        IRepository<ProductReport> prRepo, IRepository<PhysicalReport> psrRepo,
        IRepository<MaterialReport> mrRepo, IFactoryReportService frService, IHttpContextAccessor accessor,
        IMaterialReportService mrService, IOptionsSnapshot<Settings> settings,
        IRepository<SpecificationTypeRule> sptRepo,
        IRepository<CraftIndicatorRule> cirRepo, IRepository<WaterRecord> wrRepo, IRepository<MachineModel> mmRepo,
        ICraftReportService crService, IRepository<GroupRecord> grRepo, IRepository<DataRecord> drRepo,
        IRepository<FactoryReport> frRepo,
        IRepository<CraftReport> crRepo,
        IRepository<TestReport> trRepo,
        ITestReportService trService)
    {
        _gRepo = gRepo;
        _spRepo = spRepo;
        _dRepo = dRepo;
        _iRepo = iRepo;
        _prService = prService;
        _irService = irService;
        _prsService = prsService;
        _uow = uow;
        _irRepo = irRepo;
        _prRepo = prRepo;
        _psrRepo = psrRepo;
        _mrRepo = mrRepo;
        _frService = frService;
        _accessor = accessor;
        _mrService = mrService;
        _sptRepo = sptRepo;
        _cirRepo = cirRepo;
        _wrRepo = wrRepo;
        _mmRepo = mmRepo;
        _crService = crService;
        _grRepo = grRepo;
        _drRepo = drRepo;
        _frRepo = frRepo;
        _crRepo = crRepo;
        _trRepo = trRepo;
        _trService = trService;
        _settings = settings.Value;
    }

    /// <summary>
    ///     添加测量数据
    /// </summary>
    /// <param name="dto">测量数据</param>
    /// <param name="failReason">返回错误原因</param>
    /// <returns></returns>
    public bool AddDataInfo(MetricalDataEditDataDto dto, out string failReason)
    {
        failReason = string.Empty;
        var items = (JArray)JsonConvert.DeserializeObject(dto.DataInfo);
        var dataList = new List<Data>();
        foreach (var item in items)
        {
            var dataItem = new Data
            {
                GroupId = dto.GroupId,
                TestTime = Convert.ToDateTime(item["testTime"].ToString()),
                Data = item.ToString()
            };

            dataList.Add(dataItem);
        }

        _dRepo.AddRange(dataList);

        if (!_prService.Add(dto)) failReason = "生成报表失败";

        return _uow.Save() > 0;
    }

    /// <summary>
    ///     添加组
    /// </summary>
    /// <param name="dto">组数据</param>
    /// <param name="failReason">返回错误原因</param>
    /// <returns></returns>
    public bool AddGroupInfo(MetricalDataGroupEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_gRepo.All().Any(c => c.BeginTime == Convert.ToDateTime(dto.TestTime) &&
                                  c.EndTime == Convert.ToDateTime(dto.TestTime) &&
                                  c.SpecificationId == dto.SpecificationId &&
                                  c.TurnId == dto.TurnId && c.MachineModelId == dto.MachineModelId &&
                                  c.MeasureTypeId == dto.MeasureTypeId))
        {
            failReason = "该组数据已存在, 请修改选项后再提交";
            return false;
        }

        var userId = _accessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.UserData).Value;
        var group = new Group
        {
            SpecificationId = dto.SpecificationId,
            TurnId = dto.TurnId,
            MachineModelId = dto.MachineModelId,
            MeasureTypeId = dto.MeasureTypeId,
            BeginTime = Convert.ToDateTime(dto.TestTime),
            EndTime = Convert.ToDateTime(dto.TestTime),
            OrderNo = dto.OrderNo,
            Instance = dto.Instance,
            UserId = _accessor.HttpContext.getUserId(),
            UserName = _accessor.HttpContext.getUserName()
        };

        if (!string.IsNullOrEmpty(dto.ProductionTime)) group.ProductionTime = Convert.ToDateTime(dto.ProductionTime);

        if (!string.IsNullOrEmpty(dto.DeliverTime)) group.DeliverTime = Convert.ToDateTime(dto.DeliverTime);

        _gRepo.Add(group);

        var groupResult = _uow.Save() > 0;
        if (!groupResult)
        {
            failReason = "组数据保存失败";
            return false;
        }

        if (dto.CopyId > 0)
        {
            var dataList = new List<Data>();
            var waterRecordList = new List<WaterRecord>();
            var data = _dRepo.All().Where(c => c.GroupId == dto.CopyId).ToList();
            var waterRecords = _wrRepo.All().Where(c => c.GroupId == dto.CopyId).ToList();
            var offsetSeconds = 3;
            foreach (var item in data)
            {
                item.Id = 0;
                item.GroupId = group.Id;
                item.TestTime = DateTime.Now;
                var obj = JsonConvert.DeserializeObject<JObject>(item.Data);
                obj["testTime"] = DateTime.Now.AddSeconds(offsetSeconds).ToString("yyyy-MM-dd HH:mm:ss");
                item.Data = JsonConvert.SerializeObject(obj);
                dataList.Add(item);
                offsetSeconds += 3;
            }

            foreach (var waterRecord in waterRecords)
            {
                waterRecord.Id = 0;
                waterRecord.GroupId = group.Id;
                waterRecordList.Add(waterRecord);
            }

            _dRepo.AddRange(dataList);
            _wrRepo.AddRange(waterRecordList);
            return _uow.Save() > 0;
        }

        return true;
    }

    /// <summary>
    ///     更新测量数据
    /// </summary>
    /// <param name="dto">测量数据</param>
    /// <param name="failReason">返回错误原因</param>
    /// <returns></returns>
    public bool UpdateDataInfo(MetricalDataEditDataDto dto, out string failReason)
    {
        failReason = string.Empty;
        var group = _gRepo.All().Include(c => c.Specification).Include(c => c.MeasureType)
            .FirstOrDefault(c => c.Id == dto.GroupId);
        var inDbIds = _dRepo.All().Where(c => c.GroupId == dto.GroupId).Select(c => c.Id);
        var items = (JArray)JsonConvert.DeserializeObject(dto.DataInfo);
        var addList = new List<Data>();
        var updateList = new List<Data>();
        var updateIds = new List<int>();
        foreach (var item in items)
            if (item["id"] == null)
            {
                var data = new Data
                {
                    GroupId = dto.GroupId,
                    TestTime = Convert.ToDateTime(item["testTime"].ToString()),
                    Data = item.ToString()
                };
                addList.Add(data);
            }
            else
            {
                var id = Convert.ToInt32(item["id"].ToString());
                updateIds.Add(id);
                var data = _dRepo.Get(id);
                data.GroupId = dto.GroupId;
                data.TestTime = Convert.ToDateTime(item["testTime"].ToString());
                data.Data = item.ToString();
                updateList.Add(data);
            }

        var deleteIds = inDbIds.Where(c => !updateIds.Contains(c));
        var deleteList = _dRepo.All().Where(c => deleteIds.Contains(c.Id));

        _dRepo.AddRange(addList);
        _dRepo.UpdateRange(updateList);
        _dRepo.DeleteRange(deleteList);
        _uow.Save();
        if (group.MeasureTypeId == _settings.PhysicalTypeId)
        {
            if (!_prService.Add(dto)) failReason = "生成物检检验报表失败";
        }
        else if (group.MeasureTypeId == _settings.InspectionTypeId)
        {
            if (!_irService.Add(dto)) failReason = "生成巡检检验报表失败";
        }
        else if (group.MeasureTypeId == _settings.ProductionTypeId)
        {
            if (!_prsService.Add(dto)) failReason = "生成成品检验报表失败";
        }
        else if (group.MeasureTypeId == _settings.MaterialTypeId)
        {
            if (!_mrService.Add(dto)) failReason = "生产原辅材料检验报表失败";
        }
        else if (group.MeasureTypeId == _settings.FactoryTypeId)
        {
            if (!_frService.Add(dto)) failReason = "生成出厂检验报告失败";
        }
        else if (_settings.CraftTypeIds.Contains(group.MeasureTypeId))
        {
            if (!_crService.Add(dto, out var fail)) failReason = fail;
        }
        else if (group.MeasureTypeId == _settings.TestTypeId)
        {
            if (!_trService.Add(dto)) failReason = "生成测试检验报表失败";
        }

        return _uow.Save() >= 0;
    }

    /// <summary>
    ///     更新组数据信息
    /// </summary>
    /// <param name="dto">组数据</param>
    /// <param name="failReason">返回错误原因</param>
    /// <returns></returns>
    public bool UpdateGroupInfo(MetricalDataGroupEditDto dto, out string failReason)
    {
        failReason = string.Empty;
        if (_gRepo.All().Any(c => c.BeginTime == Convert.ToDateTime(dto.TestTime) &&
                                  c.EndTime == Convert.ToDateTime(dto.TestTime) &&
                                  c.SpecificationId == dto.SpecificationId &&
                                  c.TurnId == dto.TurnId && c.MachineModelId == dto.MachineModelId &&
                                  c.MeasureTypeId == dto.MeasureTypeId && c.Id != dto.Id))
        {
            failReason = "该组数据已存在, 请修改选项后再提交";
            return false;
        }

        var group = _gRepo.Get(dto.Id);
        group.SpecificationId = dto.SpecificationId;
        group.TurnId = dto.TurnId;
        group.MachineModelId = dto.MachineModelId;
        group.MeasureTypeId = dto.MeasureTypeId;
        group.BeginTime = Convert.ToDateTime(dto.TestTime);
        group.EndTime = Convert.ToDateTime(dto.TestTime);
        group.OrderNo = dto.OrderNo;
        group.Instance = dto.Instance;
        group.UserId = _accessor.HttpContext.getUserId();
        group.UserName = _accessor.HttpContext.getUserName();

        if (!string.IsNullOrEmpty(dto.ProductionTime))
            group.ProductionTime = Convert.ToDateTime(dto.ProductionTime);
        else
            group.ProductionTime = null;

        if (!string.IsNullOrEmpty(dto.DeliverTime))
            group.DeliverTime = Convert.ToDateTime(dto.DeliverTime);
        else
            group.DeliverTime = null;

        _gRepo.Update(group);

        return _uow.Save() >= 0;
    }

    /// <summary>
    ///     批量删除数据
    /// </summary>
    /// <param name="ids">组数据id数组</param>
    /// <returns></returns>
    public bool Delete(List<int> ids)
    {
        var groups = new List<Group>();
        var inspectionReports = new List<InspectionReport>();
        var productReports = new List<ProductReport>();
        var physicalReports = new List<PhysicalReport>();
        var materialReports = new List<MaterialReport>();
        var factoryReports = new List<FactoryReport>();
        var craftReports = new List<CraftReport>();
        var testReports = new List<TestReport>();
        foreach (var id in ids)
        {
            var group = _gRepo.All().Include(c => c.MeasureType).FirstOrDefault(c => c.Id == id);
            groups.Add(group);
            var typeName = group.MeasureType.Name;
            var typeId = group.MeasureTypeId;
            // 删除数据时同时将报表中对应的报表删除
            if (typeId == _settings.InspectionTypeId)
            {
                var inspectionReport = _irRepo.All().FirstOrDefault(c => c.GroupId == id);
                if (inspectionReport != null)
                    inspectionReports.Add(inspectionReport);
            }
            else if (typeId == _settings.ProductionTypeId)
            {
                var productReport = _prRepo.All().FirstOrDefault(c => c.GroupId == id);
                if (productReport != null)
                    productReports.Add(productReport);
            }
            else if (typeId == _settings.PhysicalTypeId)
            {
                var physicalReport = _psrRepo.All().FirstOrDefault(c => c.GroupId == id);
                if (physicalReport != null)
                    physicalReports.Add(physicalReport);
            }
            else if (typeId == _settings.MaterialTypeId)
            {
                var materialReport = _mrRepo.All().FirstOrDefault(c => c.GroupId == id);
                if (materialReport != null)
                    materialReports.Add(materialReport);
            }
            else if (typeId == _settings.FactoryTypeId)
            {
                var factoryReport = _frRepo.All().FirstOrDefault(c => c.GroupId == id);
                if (factoryReport != null)
                    factoryReports.Add(factoryReport);
            }
            else if (_settings.CraftTypeIds.Contains(typeId))
            {
                var craftReport = _crRepo.All().FirstOrDefault(c => c.GroupId == id);
                if (craftReport != null)
                    craftReports.Add(craftReport);
            }
            else if (typeId == _settings.TestTypeId)
            {
                var testReport = _trRepo.All().FirstOrDefault(c => c.GroupId == id);
                if (testReport != null)
                    testReports.Add(testReport);
            }
        }

        _gRepo.DeleteRange(groups);
        if (inspectionReports.Count > 0)
            _irRepo.DeleteRange(inspectionReports);
        if (productReports.Count > 0)
            _prRepo.DeleteRange(productReports);
        if (physicalReports.Count > 0)
            _psrRepo.DeleteRange(physicalReports);
        if (materialReports.Count > 0)
            _mrRepo.DeleteRange(materialReports);
        if (factoryReports.Count > 0)
            _frRepo.DeleteRange(factoryReports);
        if (craftReports.Count > 0)
            _crRepo.DeleteRange(craftReports);
        if (testReports.Count > 0)
            _trRepo.DeleteRange(testReports);

        return _uow.Save() > 0;
    }

    /// <summary>
    ///     获取数据列表
    /// </summary>
    /// <param name="dto">查询条件</param>
    /// <param name="total">返回数据总条数</param>
    /// <returns></returns>
    public IEnumerable<MetricalDataTableDto> GetTable(MetricalDataQueryDto dto, out int total)
    {
        var data = _gRepo.All().AsNoTracking();
        var roleId = _accessor.HttpContext.getUserRoleId();
        var canSeeOtherData = _accessor.HttpContext.getCanSeeOtherData();
        if (roleId != _settings.AdminTypeId)
            if (!canSeeOtherData)
            {
                var userId = _accessor.HttpContext.getUserId();
                data = data.Where(c => c.UserId == userId || c.UserId == 0);
            }

        if (!string.IsNullOrEmpty(dto.BeginTime) && !string.IsNullOrEmpty(dto.EndTime))
        {
            var beginTime = Convert.ToDateTime(dto.BeginTime);
            var endTime = Convert.ToDateTime(dto.EndTime);
            data = data.Where(c => c.BeginTime >= beginTime && c.EndTime <= endTime);
        }

        if (!string.IsNullOrEmpty(dto.Query))
            data = data.Where(c => c.Specification.Name.Contains(dto.Query) || c.Turn.Name.Contains(dto.Query) ||
                                   c.MachineModel.Name.Contains(dto.Query) || c.MeasureType.Name.Contains(dto.Query) ||
                                   c.UserName.Contains(dto.Query));

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var typeId = Convert.ToInt32(dto.SpecificationTypeId);
            data = data.Where(c => c.Specification.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = int.Parse(dto.TurnId);
            data = data.Where(c => c.TurnId == turnId);
        }

        if (!string.IsNullOrEmpty(dto.MachineModelId))
        {
            var machineModelId = int.Parse(dto.MachineModelId);
            data = data.Where(c => c.MachineModelId == machineModelId);
        }

        if (!string.IsNullOrEmpty(dto.MeasureTypeId))
        {
            var typeId = int.Parse(dto.MeasureTypeId);
            data = data.Where(c => c.MeasureTypeId == typeId);
        }

        total = data.Count();

        var result = data.Include(c => c.Specification).Include(c => c.Turn).Include(c => c.MachineModel)
            .Include(c => c.MeasureType).OrderByDescending(c => c.BeginTime).Skip(dto.Skip()).Take(dto.PageSize)
            .Select(c => new MetricalDataTableDto
            {
                Id = c.Id,
                SpecificationName = c.Specification.Name,
                SpecificationTypeId = c.Specification.SpecificationTypeId,
                TurnName = c.Turn.Name,
                MachineModelName = c.MachineModel.Name,
                MeasureTypeName = c.MeasureType.Name,
                BeginTime = c.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = c.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductionTime = c.ProductionTime == null
                    ? ""
                    : Convert.ToDateTime(c.ProductionTime).ToString("yyyy-MM-dd"),
                SpecificationId = c.SpecificationId,
                TurnId = c.TurnId,
                MachineModelId = c.MachineModelId,
                MeasureTypeId = c.MeasureTypeId,
                DeliverTime = c.DeliverTime == null ? "" : Convert.ToDateTime(c.DeliverTime).ToString("yyyy-MM-dd"),
                OrderNo = c.OrderNo ?? "",
                Instance = c.Instance,
                UserId = c.UserId,
                UserName = c.UserName
            }).ToList();

        return result;
    }

    public string GetIndicators(int id)
    {
        var specification = _spRepo.Get(id);
        var singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules).ToList();

        var columns = new JArray();
        foreach (var rule in singleRules)
        {
            var indicator = _iRepo.Get(rule.Id);
            var column = new JObject { { "prop", rule.Id }, { "label", indicator.Name } };
            var content = new JObject { { "type", "el-input" } };
            var attrs = new JObject { { "type", "number" }, { "step", "0.0000001" } };
            content.Add("attrs", attrs);
            column.Add("content", content);
            columns.Add(column);
        }

        return JsonConvert.SerializeObject(columns);
    }

    public string GetSpecification(int id)
    {
        var data = getSpecificationIndicators(id);
        return JsonConvert.SerializeObject(data);
    }

    public string GetSpecifications()
    {
        var specifications = _spRepo.All().AsNoTracking().Where(c => c.Status == Status.Enabled).ToList();
        var data = new JArray();
        foreach (var spe in specifications)
        {
            var obj = getSpecificationIndicators(spe.Id);
            data.Add(obj);
        }

        return JsonConvert.SerializeObject(data);
    }

    public string GetDataInfo(int id)
    {
        var group = _gRepo.All().Include(c => c.Specification).FirstOrDefault(c => c.Id == id);
        var data = _dRepo.All().Where(c => c.GroupId == id);
        var dataInfo = new JArray();
        foreach (var item in data)
        {
            var info = (JObject)JsonConvert.DeserializeObject(item.Data);
            if (!info.ContainsKey("id"))
                info.Add("id", item.Id);
            else
                info["id"] = item.Id;
            dataInfo.Add(info);
        }

        var temp = new
        {
            groupId = id,
            specificationName = group.Specification.Name,
            testTime = group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
            data = dataInfo
        };
        return JsonConvert.SerializeObject(temp);
    }

    public MetricalDataStatisticDto GetStatistic(int id, out string failReason)
    {
        failReason = string.Empty;
        var dto = new MetricalDataStatisticDto();
        var statisticItems = new[] { "平均值", "最大值", "最小值", "SD", "CV", "CPK", "Offs", "上超", "下超", "合格数", "合格率" };
        var group = _gRepo.Get(id);
        var specification = _spRepo.Get(group.SpecificationId);
        List<Rule> rules;
        if (group.MeasureTypeId == _settings.ChemicalTypeId)
        {
            var tempData = _sptRepo.All()
                .FirstOrDefault(c => c.SpecificationTypeId == specification.SpecificationTypeId);
            rules = JsonConvert.DeserializeObject<List<Rule>>(tempData == null
                ? specification.SingleRules
                : tempData.Rules);
        }
        else if (group.MeasureTypeId == _settings.CraftTypeId)
        {
            var temp = _cirRepo.All().FirstOrDefault(c => c.ModelId == group.MachineModelId);
            rules = JsonConvert.DeserializeObject<List<Rule>>(temp == null
                ? specification.SingleRules
                : temp.Rules);
        }
        else
        {
            rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
        }

        var data = _dRepo.All().Where(c => c.GroupId == id).Select(c => c.Data).ToList();
        if (data.Count == 0)
        {
            failReason = "没有找到测量数据, 请录入测量数据后再查看";
            return dto;
        }

        var columns = new JObject { { "itemName", new JObject { { "text", "统计项目" } } } };
        var originColumns = new JObject { { "testTime", new JObject { { "text", "测量时间" } } } };
        var dataInfo = new JArray();
        var originDataInfo = new JArray();
        foreach (var item in statisticItems) dataInfo.Add(new JObject { { "itemName", item } });

        var index = 0;
        foreach (var rule in rules)
        {
            var firstData = (JObject)JsonConvert.DeserializeObject(data.FirstOrDefault());
            // 指标标准值为 0 的为外观指标, 不需要统计数据
            if (rule.Standard == "0") continue;

            var indicatorValue = firstData[$"{rule.Id}"];
            if (indicatorValue != null && indicatorValue.ToString() != "")
            {
                var indicatorName = rule.Name;
                if (string.IsNullOrEmpty(rule.Name)) indicatorName = _iRepo.Get(rule.Id).Name;
                var column = new JObject { { "text", indicatorName } };
                columns.Add($"a{rule.Id}", column);
                originColumns.Add($"a{rule.Id}", column);
            }

            var tempDataList = new List<double>();

            foreach (var item in data)
            {
                var obj = (JObject)JsonConvert.DeserializeObject(item);
                if (index == 0)
                {
                    var tempObj = new JObject();
                    foreach (var o in obj)
                    {
                        if (o.Key == "testTime") tempObj[o.Key] = o.Value;
                        if (o.Key != "testTime" && o.Key != "id") tempObj[$"a{o.Key}"] = o.Value;
                    }

                    originDataInfo.Add(tempObj);
                }

                // 由于测量数据中可能会有没有填写所有指标的情况, 所以先判断是否存在该指标
                if (obj[$"{rule.Id}"] != null)
                {
                    var value = obj[$"{rule.Id}"].ToString();
                    if (!string.IsNullOrEmpty(value))
                        tempDataList.Add(double.Parse(value));
                }
            }

            var statistic = tempDataList.toStatistic(Convert.ToDouble(rule.Standard), Convert.ToDouble(rule.Upper),
                Convert.ToDouble(rule.Lower));
            foreach (var item in statisticItems)
            {
                var currentObj = dataInfo.First(c => c["itemName"].ToString() == item);
                var currentItem = "";
                switch (item)
                {
                    case "平均值":
                        currentItem = statistic.Mean.ToString("F2");
                        break;
                    case "最大值":
                        currentItem = statistic.Max.ToString("F2");
                        break;
                    case "最小值":
                        currentItem = statistic.Min.ToString("F2");
                        break;
                    case "SD":
                        currentItem = statistic.Sd.ToString("F3");
                        break;
                    case "CV":
                        currentItem = statistic.Cv.ToString("F3");
                        break;
                    case "CPK":
                        currentItem = statistic.Cpk.ToString("F3");
                        break;
                    case "Offs":
                        currentItem = statistic.Offset.ToString("F3");
                        break;
                    case "上超":
                        currentItem = statistic.HighCnt.ToString();
                        break;
                    case "下超":
                        currentItem = statistic.LowCnt.ToString();
                        break;
                    case "合格数":
                        currentItem = statistic.Quality;
                        break;
                    case "合格率":
                        currentItem = statistic.QualityRate;
                        break;
                }

                currentObj[$"a{rule.Id}"] = currentItem;
                index++;
            }
        }

        dto.OriginColumns = JsonConvert.SerializeObject(originColumns);
        dto.OriginDataInfo = JsonConvert.SerializeObject(originDataInfo);
        dto.StatisticColumns = JsonConvert.SerializeObject(columns);
        dto.DataInfo = JsonConvert.SerializeObject(dataInfo);
        return dto;
    }

    public IEnumerable<BaseOptionDto> GetOptions()
    {
        var data = _gRepo.All().Where(c => c.MeasureTypeId == _settings.ChemicalTypeId).Select(c => new BaseOptionDto
        {
            Value = c.Id,
            Text = $"{c.Specification.Name}({c.BeginTime:yyyy-MM-dd HH:mm:ss})"
        }).ToList();

        return data;
    }

    public IEnumerable<BaseOptionDto> GetOptions(int specificationId, DateTime testDate, int type)
    {
        var data = _gRepo.All().Where(c =>
                c.SpecificationId == specificationId && c.BeginTime.Month == testDate.Month && c.MeasureTypeId == type)
            .Select(c => new BaseOptionDto
            {
                Value = c.Id,
                Text = $"{c.Specification.Name}({c.BeginTime:yyyy-MM-dd HH:mm:ss})"
            }).ToList();

        return data;
    }

    public MemoryStream Download(MetricalDataQueryDto dto)
    {
        var data = _dRepo.All().AsNoTracking();
        if (!string.IsNullOrEmpty(dto.MeasureTypeId))
        {
            var typeId = int.Parse(dto.MeasureTypeId);
            data = data.Where(c => c.Group.MeasureTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var specificationTypeId = int.Parse(dto.SpecificationTypeId);
            data = data.Where(c => c.Group.Specification.SpecificationTypeId == specificationTypeId);
        }

        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationId = int.Parse(dto.SpecificationId);
            data = data.Where(c => c.Group.SpecificationId == specificationId);
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = int.Parse(dto.TurnId);
            data = data.Where(c => c.Group.TurnId == turnId);
        }

        if (!string.IsNullOrEmpty(dto.MachineModelId))
        {
            var machineModelId = int.Parse(dto.MachineModelId);
            data = data.Where(c => c.Group.MachineModelId == machineModelId);
        }

        if (!string.IsNullOrEmpty(dto.BeginTime) && !string.IsNullOrEmpty(dto.EndTime))
        {
            var begin = Convert.ToDateTime(dto.BeginTime);
            var end = Convert.ToDateTime(dto.EndTime);
            data = data.Where(c => c.Group.BeginTime.Date >= begin && c.Group.BeginTime.Date <= end);
        }

        var specificationGroups = data.Include(c => c.Group).ThenInclude(c => c.Specification)
            .Include(c => c.Group).ThenInclude(c => c.Turn)
            .Include(c => c.Group).ThenInclude(c => c.MachineModel)
            .Include(c => c.Group).ThenInclude(c => c.MeasureType).ToList().GroupBy(c => c.Group.SpecificationId)
            .ToList();
        var specifications = _spRepo.All().Select(c => new { c.Id, c.SingleRules }).ToList();
        var indicators = _iRepo.All().ToList();
        using var package = new ExcelPackage();

        foreach (var specificationGroup in specificationGroups)
        {
            var first = specificationGroup.First();
            if (first == null)
                continue;
            var singleRules = specifications.FirstOrDefault(c => c.Id == specificationGroup.Key);
            if (singleRules == null)
                continue;
            var ws = package.Workbook.Worksheets.Add(first.Group.Specification.Name);

            var rules = JsonConvert.DeserializeObject<List<Rule>>(singleRules.SingleRules);
            var ruleDic = new Dictionary<int, string>();
            foreach (var rule in rules)
            {
                var name = rule.Name;
                if (string.IsNullOrEmpty(name)) name = indicators.FirstOrDefault(c => c.Id == rule.Id).Name;
                ruleDic.Add(rule.Id, name);
            }

            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Column(1).Width = 25;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 25;
            ws.Column(4).Width = 25;
            ws.Cells[1, 1].Value = "测量时间";
            ws.Cells[1, 2].Value = "班次";
            ws.Cells[1, 3].Value = "机台";
            ws.Cells[1, 4].Value = "测量类型";
            var col = 5;
            foreach (var ruleItem in ruleDic)
            {
                ws.Column(col).Width = 25;
                ws.Cells[1, col].Value = ruleItem.Value;
                col++;
            }

            var row = 2;
            foreach (var item in specificationGroup.OrderBy(c => c.TestTime))
            {
                ws.Cells[row, 1].Value = item.TestTime.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cells[row, 2].Value = item.Group.Turn.Name;
                ws.Cells[row, 3].Value = item.Group.MachineModel.Name;
                ws.Cells[row, 4].Value = item.Group.MeasureType.Name;
                var valueCol = 5;
                var obj = JsonConvert.DeserializeObject<JObject>(item.Data);
                foreach (var ruleItem in ruleDic)
                {
                    var value = "";
                    if (obj[$"{ruleItem.Key}"] != null) value = obj[$"{ruleItem.Key}"].ToString();

                    ws.Cells[row, valueCol].Value = value;
                    valueCol++;
                }

                row++;
            }
        }

        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }

    public bool AddFilterData(FilterDataAddDto dto, out string failReason)
    {
        failReason = string.Empty;
        var group = _gRepo.Get(dto.GroupId);
        if (!string.IsNullOrEmpty(group.FromRecords))
        {
            var dataInDb = _dRepo.All().Where(c => c.GroupId == dto.GroupId).ToList();
            _dRepo.DeleteRange(dataInDb);
        }

        group.FromRecords = JsonConvert.SerializeObject(dto.SelectedGroupRecords);
        var groupRecordIds = dto.SelectedGroupRecords.Select(c => c.GroupId).ToList();
        var records = _grRepo.All().Where(c => groupRecordIds.Contains(c.Id)).ToList();
        var dataList = new List<Data>();
        var testTimeList = new List<DateTime>();
        var weightList = new List<double>();
        var circleList = new List<double>();
        var ovalList = new List<double>();
        var lengthList = new List<double>();
        var resistanceList = new List<double>();
        var hardnessList = new List<double>();
        foreach (var record in records)
        {
            var dataRecords = _drRepo.All().Where(c => c.GroupId == record.Id).Select(c => new
            {
                c.TestTime, c.Weight, c.Circle, c.Oval, c.Length, c.Resistance, c.Hardness
            }).ToList();
            testTimeList.AddRange(dataRecords.Select(c => c.TestTime));
            if (dataRecords.Any(c => c.Weight != null))
                weightList.AddRange(dataRecords.Select(c => Convert.ToDouble(c.Weight)));

            if (dataRecords.Any(c => c.Circle != null))
                circleList.AddRange(dataRecords.Select(c => Convert.ToDouble(c.Circle)));

            if (dataRecords.Any(c => c.Oval != null))
                ovalList.AddRange(dataRecords.Select(c => Convert.ToDouble(c.Oval)));

            if (dataRecords.Any(c => c.Length != null))
                lengthList.AddRange(dataRecords.Select(c => Convert.ToDouble(c.Length)));

            if (dataRecords.Any(c => c.Resistance != null))
                resistanceList.AddRange(dataRecords.Select(c => Convert.ToDouble(c.Resistance)));

            if (dataRecords.Any(c => c.Hardness != null))
                hardnessList.AddRange(dataRecords.Select(c => Convert.ToDouble(c.Hardness)));
        }

        var lengthArray = new[]
        {
            weightList.Count, circleList.Count, ovalList.Count, lengthList.Count, resistanceList.Count,
            hardnessList.Count
        };

        var count = lengthArray.Max();
        var distTestTimeList = testTimeList.Distinct().ToList();
        for (var i = 0; i < count; i++)
        {
            var data = new Data
            {
                GroupId = dto.GroupId,
                TestTime = DateTime.Now
            };
            var obj = new JObject();
            var testTime = new JProperty("testTime", distTestTimeList[i].ToString("yyyy-MM-dd HH:mm:ss"));
            obj.Add(testTime);

            if (weightList.Count > 0)
                if (weightList.Count > i)
                {
                    var weight = new JProperty($"{_settings.Weight}", weightList[i].ToString());
                    obj.Add(weight);
                }

            if (circleList.Count > 0)
                if (circleList.Count > i)
                {
                    var circle = new JProperty($"{_settings.Circle}", circleList[i].ToString());
                    obj.Add(circle);
                }

            if (ovalList.Count > 0)
                if (ovalList.Count > i)
                {
                    var oval = new JProperty($"{_settings.Oval}", ovalList[i].ToString());
                    obj.Add(oval);
                }

            if (lengthList.Count > 0)
                if (lengthList.Count > i)
                {
                    var length = new JProperty($"{_settings.Length}", lengthList[i].ToString());
                    obj.Add(length);
                }

            if (resistanceList.Count > 0)
                if (resistanceList.Count > i)
                {
                    var resistance = new JProperty($"{_settings.Resistance}", resistanceList[i].ToString());
                    obj.Add(resistance);
                }

            if (hardnessList.Count > 0)
                if (hardnessList.Count > i)
                {
                    var hardness = new JProperty($"{_settings.Hardness}", hardnessList[i].ToString());
                    obj.Add(hardness);
                }

            data.Data = JsonConvert.SerializeObject(obj);
            dataList.Add(data);
        }

        _dRepo.AddRange(dataList);

        var result = _uow.Save() > 0;
        return result;
    }

    public IEnumerable<BaseOptionDto> GetMeasureDataBySpecificationIdAndMeasureTypeId(int specificationId,
        int measureTypeId)
    {
        var groups = _gRepo.All().Include(c => c.Specification)
            .Where(c => c.SpecificationId == specificationId && c.MeasureTypeId == measureTypeId)
            .OrderByDescending(c => c.BeginTime).Select(c =>
                new BaseOptionDto
                {
                    Value = c.Id, Text = $"{c.Specification.Name}( {c.BeginTime:yyyy-MM-dd HH:mm:ss} )"
                }).ToList();

        return groups;
    }

    private JObject getSpecificationIndicators(int id)
    {
        var group = _gRepo.Get(id);
        var specification = _spRepo.Get(group.SpecificationId);
        var label = new JObject { { "label", "原始数据" }, { "type", "table-editor" } };
        var columns = new JArray();
        List<Rule> rules;
        if (group.MeasureTypeId == _settings.ChemicalTypeId)
        {
            var spt = _sptRepo.All().FirstOrDefault(c => c.SpecificationTypeId == specification.SpecificationTypeId);
            rules = JsonConvert.DeserializeObject<List<Rule>>(spt != null ? spt.Rules : specification.SingleRules);
        }
        else if (_settings.CraftTypeIds.Contains(group.MeasureTypeId))
        {
            var machineModel = _mmRepo.Get(group.MachineModelId);
            CraftIndicatorRule craftRules = null;
            if (machineModel.ModelId > 0)
                craftRules = _cirRepo.All().FirstOrDefault(c => c.ModelId == machineModel.ModelId);

            rules = JsonConvert.DeserializeObject<List<Rule>>(craftRules != null
                ? craftRules.Rules
                : specification.SingleRules);
        }
        else
        {
            rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
        }

        var idObj = new JObject { { "prop", "id" }, { "label", "ID" }, { "width", "150" } };
        columns.Add(idObj);
        var testTime = new JObject { { "prop", "testTime" }, { "label", "测量时间" }, { "width", "300" } };
        var testTimeContent = new JObject { { "type", "el-date-picker" } };
        var testTimeAttrs = new JObject
        {
            { "type", "datetime" }, { "value-format", "yyyy-MM-dd HH:mm:ss" },
            { "default-value", group.BeginTime.Date },
            { "default-time", $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}" }
        };
        testTimeContent.Add("attrs", testTimeAttrs);
        testTime.Add("content", testTimeContent);
        columns.Add(testTime);
        var indicators = _iRepo.All().AsNoTracking();
        foreach (var rule in rules.OrderByDescending(c => c.Standard))
        {
            var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
            if (indicator == null) continue;
            var indicatorName = indicator.Name;
            if (rule.Standard == "0") indicatorName += "(外观)";
            var column = new JObject { { "prop", rule.Id.ToString() }, { "label", indicatorName }, { "width", 150 } };
            var content = new JObject { { "type", "el-input" } };
            var attrs = new JObject { { "type", "number" }, { "step", "0.0000001" } };
            content.Add("attrs", attrs);
            column.Add("content", content);
            columns.Add(column);
        }

        var tableAttrs = new JObject { { "ref", "dataTable" }, { "height", "500" } };
        var parentAttrs = new JObject { { "tableAttrs", tableAttrs }, { "columns", columns } };
        label.Add("attrs", parentAttrs);
        var dataObj = new JObject { { "data", label } };
        var specificationObj = new JObject { { "id", specification.Id }, { "desc", dataObj } };

        return specificationObj;
    }
}