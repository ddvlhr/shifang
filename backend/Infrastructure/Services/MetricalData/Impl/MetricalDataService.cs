using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Dtos.MetricalData;
using Core.Entities;
using Core.SugarEntities;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SqlSugar;
using Machine = Core.SugarEntities.Machine;
using MeasureType = Core.SugarEntities.MeasureType;
using Team = Core.SugarEntities.Team;
using Turn = Core.SugarEntities.Turn;
using System.Threading.Tasks;
using System.Xml;
using Core.Dtos.Statistics;
using Core.Enums;
using ShiFangSettings = Core.Models.ShiFangSettings;
using RTLib;
using EquipmentType = Core.Enums.EquipmentType;
using static Core.Dtos.DashboardDto;

namespace Infrastructure.Services.MetricalData.Impl;

public class MetricalDataService : SugarRepository<MetricalGroup>, IMetricalDataService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IRepository<CraftReport> _crRepo;
    private readonly IRepository<TestReport> _trRepo;
    private readonly ISqlSugarClient _db;
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<FactoryReport> _frRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<Core.Entities.Indicator> _iRepo;
    private readonly IRepository<InspectionReport> _irRepo;
    private readonly IRepository<MaterialReport> _mrRepo;
    private readonly IRepository<ProductReport> _prRepo;
    private readonly IRepository<PhysicalReport> _psrRepo;
    private readonly ShiFangSettings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<WaterRecord> _wrRepo;

    public MetricalDataService(IRepository<Group> gRepo, IRepository<Core.Entities.Specification> spRepo,
        IRepository<Data> dRepo, IRepository<Core.Entities.Indicator> iRepo,
        IUnitOfWork uow, IRepository<InspectionReport> irRepo,
        IRepository<ProductReport> prRepo, IRepository<PhysicalReport> psrRepo,
        IRepository<MaterialReport> mrRepo, IHttpContextAccessor accessor,
        IOptionsSnapshot<ShiFangSettings> settings, IRepository<WaterRecord> wrRepo,
        IRepository<FactoryReport> frRepo,
        IRepository<CraftReport> crRepo,
        IRepository<TestReport> trRepo,
        ISqlSugarClient db)
    {
        _gRepo = gRepo;
        _spRepo = spRepo;
        _dRepo = dRepo;
        _iRepo = iRepo;
        _uow = uow;
        _irRepo = irRepo;
        _prRepo = prRepo;
        _psrRepo = psrRepo;
        _mrRepo = mrRepo;
        _accessor = accessor;
        _wrRepo = wrRepo;
        _frRepo = frRepo;
        _crRepo = crRepo;
        _trRepo = trRepo;
        _db = db;
        _settings = settings.Value;
    }

    /// <summary>
    /// 添加测量数据
    /// </summary>
    /// <param name="dto">测量数据</param>
    /// <param name="failReason">返回错误原因</param>
    /// <returns></returns>
    public bool AddDataInfo(MetricalDataEditDataDto dto, out string failReason)
    {
        //TODO 添加测量数据时会重复添加
        failReason = string.Empty;
        var items = (JArray) JsonConvert.DeserializeObject(dto.DataInfo);
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

        // if (!_prService.Add(dto)) failReason = "生成报表失败";

        return _uow.Save() > 0;
    }

    /// <summary>
    /// 添加组
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
                                  c.TurnId == dto.TurnId && c.MachineId == dto.MachineId &&
                                  c.MeasureTypeId == dto.MeasureTypeId))
        {
            failReason = "该组数据已存在, 请修改选项后再提交";
            return false;
        }

        var userId = _accessor.HttpContext.getUserId();
        var group = new Group
        {
            SpecificationId = dto.SpecificationId,
            TurnId = dto.TurnId,
            TeamId = dto.TeamId,
            MachineId = dto.MachineId,
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
    /// 更新测量数据
    /// </summary>
    /// <param name="dto">测量数据</param>
    /// <param name="failReason">返回错误原因</param>
    /// <returns></returns>
    public bool UpdateDataInfo(MetricalDataEditDataDto dto, out string failReason)
    {
        failReason = string.Empty;
        var items = JsonConvert.DeserializeObject<JArray>(dto.DataInfo);
        var inDbIds = _db.Queryable<Core.SugarEntities.MetricalData>().Where(c => c.GroupId == dto.GroupId)
            .Select(c => c.Id).ToList();
        if (items == null)
        {
            failReason = "数据格式不正确";
            return false;
        }

        var updateIds = new List<int>();
        var dataList = new List<Core.SugarEntities.MetricalData>();
        foreach (var item in items)
            if (item["id"] == null)
            {
                var data = new Core.SugarEntities.MetricalData
                {
                    GroupId = dto.GroupId,
                    TestTime = Convert.ToDateTime(item["testTime"]?.ToString()),
                    Data = item.ToString()
                };
                // addList.Add(data);
                dataList.Add(data);
            }
            else
            {
                var id = Convert.ToInt32(item["id"].ToString());
                updateIds.Add(id);
                // var data = _dRepo.Get(id);
                var data = _db.Queryable<Core.SugarEntities.MetricalData>().First(c => c.Id == id);
                data.GroupId = dto.GroupId;
                data.TestTime = Convert.ToDateTime(item["testTime"]?.ToString());
                data.Data = item.ToString();
                // updateList.Add(data);
                dataList.Add(data);
            }

        var deleteIds = inDbIds.Where(c => !updateIds.Contains(c))
            .Select(c => new Core.SugarEntities.MetricalData {Id = c}).ToList();
        // var deleteList = _dRepo.All().Where(c => deleteIds.Contains(c.Id));

        var ret = _db.Storageable(dataList).ExecuteCommand();
        _db.Deleteable<Core.SugarEntities.MetricalData>(deleteIds).ExecuteCommand();

        return ret >= 0;
    }

    /// <summary>
    ///     更新组数据信息
    /// </summary>
    /// <param name="dto">组数据</param>
    /// <param name="failReason">返回错误原因</param>
    /// <returns></returns>
    public bool UpdateGroupInfo(MetricalDataGroupEditDto dto, out int groupId, out string failReason)
    {
        groupId = 0;
        failReason = string.Empty;

        var modify = dto.Id > 0;
        var group = new MetricalGroup()
        {
            Id = dto.Id,
            BeginTime = Convert.ToDateTime(dto.TestTime),
            EndTime = Convert.ToDateTime(dto.TestTime),
            SpecificationId = dto.SpecificationId,
            TeamId = dto.TeamId,
            TurnId = dto.TurnId,
            MachineId = dto.MachineId,
            MeasureTypeId = dto.MeasureTypeId,
            OrderNo = dto.OrderNo,
            Instance = dto.Instance,
            UserName = _accessor.HttpContext.getUserName()
        };

        if (!string.IsNullOrEmpty(dto.ProductionTime))
            group.ProductionTime = Convert.ToDateTime(dto.ProductionTime);
        else
            group.ProductionTime = null;

        if (!string.IsNullOrEmpty(dto.DeliverTime))
            group.DeliverTime = Convert.ToDateTime(dto.DeliverTime);
        else
            group.DeliverTime = null;

        var ret = _db.Storageable<MetricalGroup>(group).ExecuteCommand() > 0;
        if (ret) groupId = group.Id;
        return ret;
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
            // if (typeId == _settings.InspectionTypeId)
            // {
            //     var inspectionReport = _irRepo.All().FirstOrDefault(c => c.GroupId == id);
            //     if (inspectionReport != null)
            //         inspectionReports.Add(inspectionReport);
            // }
            // else if (typeId == _settings.ProductionTypeId)
            // {
            //     var productReport = _prRepo.All().FirstOrDefault(c => c.GroupId == id);
            //     if (productReport != null)
            //         productReports.Add(productReport);
            // }
            // else if (typeId == _settings.PhysicalTypeId)
            // {
            //     var physicalReport = _psrRepo.All().FirstOrDefault(c => c.GroupId == id);
            //     if (physicalReport != null)
            //         physicalReports.Add(physicalReport);
            // }
            // else if (typeId == _settings.MaterialTypeId)
            // {
            //     var materialReport = _mrRepo.All().FirstOrDefault(c => c.GroupId == id);
            //     if (materialReport != null)
            //         materialReports.Add(materialReport);
            // }
            // else if (typeId == _settings.FactoryTypeId)
            // {
            //     var factoryReport = _frRepo.All().FirstOrDefault(c => c.GroupId == id);
            //     if (factoryReport != null)
            //         factoryReports.Add(factoryReport);
            // }
            // else if (_settings.CraftTypeIds.Contains(typeId))
            // {
            //     var craftReport = _crRepo.All().FirstOrDefault(c => c.GroupId == id);
            //     if (craftReport != null)
            //         craftReports.Add(craftReport);
            // }
            // else if (typeId == _settings.TestTypeId)
            // {
            //     var testReport = _trRepo.All().FirstOrDefault(c => c.GroupId == id);
            //     if (testReport != null)
            //         testReports.Add(testReport);
            // }
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
        total = 0;
        var departmentType = _accessor.HttpContext.getDepartmentType();
        var equipmentTypes = dto.EquipmentTypeId == null
            ? new List<EquipmentType>()
            : new List<EquipmentType> {(EquipmentType) int.Parse(dto.EquipmentTypeId)};
        if (departmentType == DepartmentType.Cigarette)
        {
            equipmentTypes = new List<EquipmentType>() {EquipmentType.Mts, EquipmentType.Rt};
        }
        else if (departmentType == DepartmentType.Cigar)
        {
            equipmentTypes = new List<EquipmentType>() {EquipmentType.SingleResistance};
        }

        var exp = Expressionable.Create<MetricalGroup>()
            .AndIF(dto.SpecificationId != null, c => c.SpecificationId == int.Parse(dto.SpecificationId))
            .AndIF(dto.BeginTime != null && dto.EndTime != null,
                c => c.BeginTime.Date >= Convert.ToDateTime(dto.BeginTime).Date &&
                     c.BeginTime.Date <= Convert.ToDateTime(dto.EndTime).Date)
            .AndIF(dto.SpecificationTypeId != null,
                c => c.Specification.SpecificationTypeId == int.Parse(dto.SpecificationTypeId))
            .AndIF(dto.TurnId != null, c => c.TurnId == int.Parse(dto.TurnId))
            .AndIF(dto.MeasureTypeId != null, c => c.MeasureTypeId == int.Parse(dto.MeasureTypeId))
            .AndIF(dto.MachineModelId != null, c => c.MachineId == int.Parse(dto.MachineModelId))
            .AndIF(equipmentTypes.Count > 0, c => equipmentTypes.Contains(c.EquipmentType))
            .ToExpression();
        var list = _db.Queryable<MetricalGroup>()
            .LeftJoin<Core.SugarEntities.Specification>((c, s) => c.SpecificationId == s.Id)
            .LeftJoin<Team>((c, s, team) => c.TeamId == team.Id)
            .LeftJoin<Turn>((c, s, team, turn) => c.TurnId == turn.Id)
            .LeftJoin<Machine>((c, s, team, turn, mac) => c.MachineId == mac.Id)
            .LeftJoin<MeasureType>((c, s, team, turn, mac, mt) => c.MeasureTypeId == mt.Id)
            .Where(exp)
            .OrderByDescending(c => c.BeginTime)
            .Select((c, s, team, turn, mac, mt) => new MetricalDataTableDto()
            {
                Id = c.Id,
                SpecificationId = c.SpecificationId,
                SpecificationName = s.Name,
                SpecificationTypeId = c.Specification.SpecificationTypeId,
                TeamId = c.TeamId,
                TeamName = team.Name,
                TurnName = turn.Name,
                MachineId = c.MachineId,
                MachineName = mac.Name,
                MeasureTypeName = mt.Name,
                BeginTime = c.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = c.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductionTime = c.ProductionTime == null
                    ? ""
                    : Convert.ToDateTime(c.ProductionTime).ToString("yyyy-MM-dd"),
                TurnId = c.TurnId,
                MeasureTypeId = c.MeasureTypeId,
                DeliverTime = c.DeliverTime == null ? "" : Convert.ToDateTime(c.DeliverTime).ToString("yyyy-MM-dd"),
                OrderNo = c.OrderNo ?? "",
                Instance = c.Instance,
                UserName = c.UserName,
                EquipmentType = c.EquipmentType,
                UserData = c.UserData
            })
            .ToPageList(dto.PageNum, dto.PageSize, ref total);

        foreach (var item in list)
        {
            item.EquipmentTypeName = item.EquipmentType.toDescription();
            if (item.EquipmentType == EquipmentType.SingleResistance)
            {
                item.UserName = item.UserData;
            }
        }

        return list;
    }

    public string GetIndicators(int id)
    {
        var specification = _spRepo.Get(id);
        var singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules).ToList();

        var columns = new JArray();
        foreach (var rule in singleRules)
        {
            var indicator = _iRepo.Get(rule.Id);
            var column = new JObject {{"prop", rule.Id}, {"label", indicator.Name}};
            var content = new JObject {{"type", "el-input"}};
            var attrs = new JObject {{"type", "number"}, {"step", "0.0000001"}};
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

    public List<Dictionary<string, object>> GetDataInfoByGroupId(int groupId)
    {
        var list = new List<Dictionary<string, object>>();
        if (groupId > 0)
        {
            var data = _dRepo.All().Where(c => c.GroupId == groupId).ToList();
            foreach (var item in data)
            {
                var info = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Data);
                if (info == null) continue;
                if (!info.ContainsKey("id"))
                    info.Add("id", item.Id);
                else
                    info["id"] = item.Id;
                list.Add(info);
            }

            return list;
        }
        else
        {
            return list;
        }
    }

    public string GetDataInfo(int id)
    {
        var group = _db.Queryable<MetricalGroup>()
            .LeftJoin<Core.SugarEntities.Specification>((c, s) => c.SpecificationId == s.Id)
            .Where(c => c.Id == id)
            .Select((c, s) => new
            {
                beginTime = c.BeginTime,
                specificationName = s.Name
            }).First();
        var data = _db.Queryable<Core.SugarEntities.MetricalData>().Where(c => c.GroupId == id).ToList();
        var dataInfo = new JArray();
        foreach (var item in data)
        {
            var info = JObject.Parse(item.Data);
            if (!info.TryGetValue("id", out JToken token))
                info.Add("id", item.Id);
            else
                token.Replace(item.Id);
            dataInfo.Add(info);
        }

        var temp = new
        {
            groupId = id,
            specificationName = group.specificationName,
            testTime = group.beginTime.ToString("yyyy-MM-dd HH:mm:ss"),
            data = dataInfo
        };
        return JsonConvert.SerializeObject(temp);
    }

    public BaseStatisticInfoDto GetStatisticInfo(int groupId, bool dayStatistic, out string failReason)
    {
        var info = new BaseStatisticInfoDto();
        failReason = "";
        var indicators = _iRepo.All().ToList();
        var statisticItems = new[] {"平均值", "最大值", "最小值", "SD", "CV", "CPK", "Offs", "上超", "下超", "合格数", "合格率"};
        var group = _db.Queryable<MetricalGroup>().Includes(c=>c.Specification).Includes(c=>c.Machine).Single(c => c.Id == groupId);
        if (group == null)
        {
            failReason = "没有找到对应的测试组";
            return info;
        }

        var specification = _db.Queryable<Core.SugarEntities.Specification>()
            .Single(c => c.Id == group.SpecificationId);
        if (specification == null)
        {
            failReason = "没有找到对应的牌号";
            return info;
        }

        var rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
        if (rules == null)
        {
            failReason = "没有找到对应的牌号规则";
            return info;
        }

        info.SpecificationName = group.Specification.Name;
        info.MachineId = group.MachineId;
        info.Instance = group.EquipmentType == EquipmentType.SingleResistance ? group.Instance : group.Machine.Name;
        info.BeginTime = group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
        if (!string.IsNullOrEmpty(group.Instance))
        {
            var wsName = group.Instance[..1];
            info.WorkShopName = wsName switch
            {
                "J" => "甲班",
                "Y" => "乙班",
                "B" => "丙班",
                _ => ""
            };
        }
        else
        {
            info.WorkShopName = "";
        }

        var data = _db.Queryable<Core.SugarEntities.MetricalData>().Where(c => c.GroupId == groupId)
            .Select(c => c.Data).ToList();
        var specificationDataList = new List<string>();
        if (dayStatistic)
        {
            var groupIds = _db.Queryable<MetricalGroup>().Where(c =>
                c.SpecificationId == group.SpecificationId &&
                c.Instance == group.Instance &&
                c.BeginTime.Date >= group.BeginTime.Date &&
                c.EndTime.Date <= group.EndTime.Date).Select(c => c.Id).ToList();
            specificationDataList = _db.Queryable<Core.SugarEntities.MetricalData>()
                .Where(c => groupIds.Contains(c.GroupId)).Select(c => c.Data).ToList();
        }

        if (data.Count == 0)
        {
            failReason = "没有找到对应的测试数据";
            return info;
        }

        var ruleDics = new Dictionary<string, DoubleRule>();
        var columns = new Dictionary<string, object>()
            {{"testTime", new Dictionary<string, object>() {{"text", "测量时间"}}}};
        var statisticColumns = new Dictionary<string, object>();
        var dataInfo = new List<Dictionary<string, object>>();
        var statisticDataInfo = new List<Dictionary<string, Dictionary<string, object>>>();
        var dayStatisticDataInfo = new List<Dictionary<string, Dictionary<string, object>>>();
        var chartDataInfo = new Dictionary<string, List<double>>();
        var chartMarkLineInfo = new Dictionary<string, Dictionary<string, int>>();
        foreach (var statisticItem in statisticItems)
        {
            var dic = new Dictionary<string, Dictionary<string, object>>()
            {
                {"itemName", new Dictionary<string, object>() {{"text", statisticItem}, {"high", 0}, {"low", 0}}}
            };
            statisticDataInfo.Add(dic);
            var dic2 = new Dictionary<string, Dictionary<string, object>>()
            {
                {"itemName", new Dictionary<string, object>() {{"text", statisticItem}, {"high", 0}, {"low", 0}}}
            };
            dayStatisticDataInfo.Add(dic2);
        }

        var index = 0;
        foreach (var rule in rules)
        {
            var firstDataStr = data.FirstOrDefault();
            if (firstDataStr == null) continue;
            var firstData = JsonConvert.DeserializeObject<JObject>(firstDataStr);
            if (firstData == null) continue;

            if (rule.Standard == "0") continue;

            var indicatorValue = firstData[$"{rule.Id}"];
            if (indicatorValue != null && indicatorValue.ToString() != "")
            {
                var indicatorName = indicators.FirstOrDefault(c => c.Id == rule.Id)?.Name;
                var columnItem = new Dictionary<string, object>()
                {
                    {"text", indicatorName}
                };
                columns.Add($"a{rule.Id}", columnItem);
                statisticColumns.Add($"a{rule.Id}", columnItem);
            }

            var dataList = new List<double>();
            foreach (var item in data)
            {
                var dataItem = JsonConvert.DeserializeObject<JObject>(item);
                if (dataItem == null) continue;
                if (index == 0)
                {
                    var temp = new Dictionary<string, object>();
                    foreach (var kv in dataItem)
                    {
                        if (kv.Key == "testTime")
                        {
                            temp.Add(kv.Key, new {text = kv.Value?.ToString(), high = 0, low = 0});
                        }

                        if (kv.Key != "testTime" && kv.Key != "id")
                        {
                            var indicatorId = Convert.ToInt32(kv.Key);
                            var currentRule = rules.FirstOrDefault(c => c.Id == indicatorId);
                            var standard = Convert.ToDouble(currentRule?.Standard);
                            var upper = Convert.ToDouble(currentRule?.Upper);
                            var lower = Convert.ToDouble(currentRule?.Lower);
                            var tempInfo = new
                            {
                                text = Convert.ToDouble(kv.Value?.ToString()),
                                high = standard + upper,
                                low = standard - lower
                            };
                            temp.Add($"a{kv.Key}", tempInfo);
                        }
                    }

                    dataInfo.Add(temp);
                }

                // 由于测量数据中可能会有没有填写所有指标的情况, 所以先判断是否存在该指标
                var currentDataItem = dataItem[$"{rule.Id}"];
                if (currentDataItem == null) continue;
                var value = currentDataItem.ToString();
                if (!string.IsNullOrEmpty(value))
                    dataList.Add(double.Parse(value));
            }

            var specificationDataItemList = new List<double>();
            if (dayStatistic)
            {
                foreach (var item in specificationDataList)
                {
                    var dataItem = JsonConvert.DeserializeObject<JObject>(item);
                    if (dataItem == null) continue;

                    // 由于测量数据中可能会有没有填写所有指标的情况, 所以先判断是否存在该指标
                    var currentDataItem = dataItem[$"{rule.Id}"];
                    if (currentDataItem == null) continue;
                    var value = currentDataItem.ToString();
                    if (!string.IsNullOrEmpty(value))
                        specificationDataItemList.Add(double.Parse(value));
                }
            }

            var ruleStandard = Convert.ToDouble(rule.Standard);
            var ruleUpper = Convert.ToDouble(rule.Upper);
            var ruleLower = Convert.ToDouble(rule.Lower);
            var ruleQualityUpper = string.IsNullOrEmpty(rule.QualityUpper) ? 0 : Convert.ToDouble(rule.QualityUpper);
            var ruleQualityLower = string.IsNullOrEmpty(rule.QualityLower) ? 0 : Convert.ToDouble(rule.QualityLower);
            var doubleRule = new DoubleRule()
            {
                Standard = ruleStandard,
                Upper = ruleStandard + ruleUpper,
                Lower = ruleStandard - ruleLower,
                QualityUpper = ruleStandard + ruleQualityUpper,
                QualityLower = ruleStandard - ruleQualityLower
            };

            var chartMarkLineTemp = new Dictionary<string, int>()
                {{"upper", 0}, {"lower", 0}, {"qualityUpper", 0}, {"qualityLower", 0}, {"standard", 0}};
            foreach (var d in dataList)
            {
                if (!chartMarkLineInfo.TryGetValue(rule.Id.ToString(), out chartMarkLineTemp))
                {
                    chartMarkLineTemp = new Dictionary<string, int>()
                        {{"upper", 0}, {"lower", 0}, {"qualityUpper", 0}, {"qualityLower", 0}, {"standard", 0}};
                    chartMarkLineInfo.Add(rule.Id.ToString(), chartMarkLineTemp);
                }

                if (d > doubleRule.Upper)
                {
                    chartMarkLineTemp["upper"] += 1;
                }
                else if (d < doubleRule.Lower)
                {
                    chartMarkLineTemp["lower"] += 1;
                }
                else if (doubleRule.QualityUpper != doubleRule.Standard && d > doubleRule.QualityUpper &&
                         d < doubleRule.Upper)
                {
                    chartMarkLineTemp["qualityUpper"] += 1;
                }
                else if (doubleRule.QualityLower != doubleRule.Standard && d < doubleRule.QualityLower &&
                         d > doubleRule.Lower)
                {
                    chartMarkLineTemp["qualityLower"] += 1;
                }
                else
                {
                    chartMarkLineTemp["standard"] += 1;
                }
            }

            chartMarkLineInfo[rule.ToString()] = chartMarkLineTemp;

            ruleDics.Add(rule.Id.ToString(), doubleRule);
            chartDataInfo.Add(rule.Id.ToString(), dataList);

            var statistic = dataList.toStatistic(ruleStandard, ruleUpper,
                ruleLower, ruleQualityUpper, ruleQualityLower);
            var dayStatisticInfo = new StatisticItem();
            if (dayStatistic)
                dayStatisticInfo = specificationDataItemList.toStatistic(ruleStandard, ruleUpper, ruleLower,
                    ruleQualityUpper, ruleQualityLower);
            foreach (var item in statisticItems)
            {
                //var currentObj = statisticDataInfo.Where(c => c.Value == item);
                var current = statisticDataInfo.FirstOrDefault(c => c["itemName"]["text"].ToString() == item);
                if (current == null) continue;
                var currentItem = item switch
                {
                    "平均值" => statistic.Mean.toString(_settings.IndicatorDecimal.Mean),
                    "最大值" => statistic.Max.toString(_settings.IndicatorDecimal.Max),
                    "最小值" => statistic.Min.toString(_settings.IndicatorDecimal.Min),
                    "SD" => statistic.Sd.toString(_settings.IndicatorDecimal.Sd),
                    "CV" => statistic.Cv.toString(_settings.IndicatorDecimal.Cv),
                    "CPK" => statistic.Cpk.toString(_settings.IndicatorDecimal.Cpk),
                    "Offs" => statistic.Offset.toString(_settings.IndicatorDecimal.Offs),
                    "上超" => statistic.HighCnt.ToString(),
                    "下超" => statistic.LowCnt.ToString(),
                    "合格数" => statistic.Quality,
                    "合格率" => statistic.QualityRate,
                    _ => ""
                };

                var spCurrent = new Dictionary<string, Dictionary<string, object>>();
                var spCurrentItem = "";
                if (dayStatistic)
                {
                    spCurrent = dayStatisticDataInfo.FirstOrDefault(c => c["itemName"]["text"].ToString() == item);
                    if (spCurrent == null) continue;
                    spCurrentItem = item switch
                    {
                        "平均值" => dayStatisticInfo.Mean.toString(_settings.IndicatorDecimal.Mean),
                        "最大值" => dayStatisticInfo.Max.toString(_settings.IndicatorDecimal.Max),
                        "最小值" => dayStatisticInfo.Min.toString(_settings.IndicatorDecimal.Min),
                        "SD" => dayStatisticInfo.Sd.toString(_settings.IndicatorDecimal.Sd),
                        "CV" => dayStatisticInfo.Cv.toString(_settings.IndicatorDecimal.Cv),
                        "CPK" => dayStatisticInfo.Cpk.toString(_settings.IndicatorDecimal.Cpk),
                        "Offs" => dayStatisticInfo.Offset.toString(_settings.IndicatorDecimal.Offs),
                        "上超" => dayStatisticInfo.HighCnt.ToString(),
                        "下超" => dayStatisticInfo.LowCnt.ToString(),
                        "合格数" => dayStatisticInfo.Quality,
                        "合格率" => dayStatisticInfo.QualityRate,
                        _ => ""
                    };
                }

                var baseInfoItems = new[] {"平均值", "最大值", "最小值"};
                var itemInfo = new Dictionary<string, object>();
                var spItemInfo = new Dictionary<string, object>();
                if (baseInfoItems.Contains(item))
                {
                    var standard = Convert.ToDouble(rule.Standard);
                    var upper = Convert.ToDouble(rule.Upper);
                    var lower = Convert.ToDouble(rule.Lower);
                    itemInfo.Add("text", currentItem);
                    itemInfo.Add("high", standard + upper);
                    itemInfo.Add("low", standard - lower);
                    if (dayStatistic)
                    {
                        spItemInfo.Add("text", spCurrentItem);
                        spItemInfo.Add("high", standard + upper);
                        spItemInfo.Add("low", standard - lower);
                    }
                }
                else
                {
                    itemInfo.Add("text", currentItem);
                    itemInfo.Add("high", 0);
                    itemInfo.Add("low", 0);
                    if (dayStatistic)
                    {
                        spItemInfo.Add("text", spCurrentItem);
                        spItemInfo.Add("high", 0);
                        spItemInfo.Add("low", 0);
                    }
                }

                current.Add($"a{rule.Id}", itemInfo);
                if (dayStatistic)
                    spCurrent.Add($"a{rule.Id}", spItemInfo);
                index++;
            }
        }

        info.Columns = columns;
        info.StatisticColumns = statisticColumns;
        info.DataInfo = dataInfo;
        info.StatisticDataInfo = statisticDataInfo;
        info.Standard = ruleDics;
        info.ChartDataInfo = chartDataInfo;
        info.DayStatisticDataInfo = dayStatisticDataInfo;
        info.ChartMarkLineInfo = chartMarkLineInfo;

        return info;
    }

    public MetricalDataStatisticDto GetStatistic(int id, out string failReason)
    {
        failReason = string.Empty;
        var dto = new MetricalDataStatisticDto();
        var statisticItems = new[] {"平均值", "最大值", "最小值", "SD", "CV", "CPK", "Offs", "上超", "下超", "合格数", "合格率"};
        var group = _gRepo.Get(id);
        var specification = _spRepo.Get(group.SpecificationId);
        var rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);


        var data = _dRepo.All().Where(c => c.GroupId == id).Select(c => c.Data).ToList();
        if (data.Count == 0)
        {
            failReason = "没有找到测量数据, 请录入测量数据后再查看";
            return dto;
        }

        var columns = new JObject {{"itemName", new JObject {{"text", "统计项目"}}}};
        var originColumns = new JObject {{"testTime", new JObject {{"text", "测量时间"}}}};
        var dataInfo = new JArray();
        var originDataInfo = new JArray();
        foreach (var item in statisticItems) dataInfo.Add(new JObject {{"itemName", item}});

        var index = 0;
        foreach (var rule in rules)
        {
            var firstData = (JObject) JsonConvert.DeserializeObject(data.FirstOrDefault());
            // 指标标准值为 0 的为外观指标, 不需要统计数据
            if (rule.Standard == "0") continue;

            var indicatorValue = firstData[$"{rule.Id}"];
            if (indicatorValue != null && indicatorValue.ToString() != "")
            {
                var indicatorName = rule.Name;
                if (string.IsNullOrEmpty(rule.Name)) indicatorName = _iRepo.Get(rule.Id).Name;
                var column = new JObject {{"text", indicatorName}};
                columns.Add($"a{rule.Id}", column);
                originColumns.Add($"a{rule.Id}", column);
            }

            var tempDataList = new List<double>();

            foreach (var item in data)
            {
                var obj = (JObject) JsonConvert.DeserializeObject(item);
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
                var currentItem = item switch
                {
                    "平均值" => statistic.Mean.toString(_settings.IndicatorDecimal.Mean),
                    "最大值" => statistic.Max.toString(_settings.IndicatorDecimal.Max),
                    "最小值" => statistic.Min.toString(_settings.IndicatorDecimal.Min),
                    "SD" => statistic.Sd.toString(_settings.IndicatorDecimal.Sd),
                    "CV" => statistic.Cv.toString(_settings.IndicatorDecimal.Cv),
                    "CPK" => statistic.Cpk.toString(_settings.IndicatorDecimal.Cpk),
                    "Offs" => statistic.Offset.toString(_settings.IndicatorDecimal.Offs),
                    "上超" => statistic.HighCnt.ToString(),
                    "下超" => statistic.LowCnt.ToString(),
                    "合格数" => statistic.Quality,
                    "合格率" => statistic.QualityRate,
                    _ => ""
                };

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
        return null;
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
        var exp = Expressionable
            .Create<Core.SugarEntities.MetricalData, MetricalGroup, Core.SugarEntities.Specification, MeasureType, Turn,
                Machine>()
            .AndIF(dto.SpecificationId != null,
                (c, g, s, mt, turn, mac) => g.SpecificationId == int.Parse(dto.SpecificationId))
            .AndIF(dto.BeginTime != null && dto.EndTime != null,
                (c, g, s, mt, turn, mac) => g.BeginTime.Date >= Convert.ToDateTime(dto.BeginTime).Date &&
                                            g.BeginTime.Date <= Convert.ToDateTime(dto.EndTime).Date)
            .AndIF(dto.SpecificationTypeId != null,
                (c, g, s, mt, turn, mac) => s.SpecificationTypeId == int.Parse(dto.SpecificationTypeId))
            .AndIF(dto.TurnId != null, (c, g, s, mt, turn, mac) => g.TurnId == int.Parse(dto.TurnId))
            .AndIF(dto.MeasureTypeId != null,
                (c, g, s, mt, turn, mac) => g.MeasureTypeId == int.Parse(dto.MeasureTypeId))
            .AndIF(dto.MachineModelId != null, (c, g, s, mt, turn, mac) => g.MachineId == int.Parse(dto.MachineModelId))
            .AndIF(dto.EquipmentTypeId != null,
                (c, g, s, mt, turn, mac) => g.EquipmentType == (EquipmentType) int.Parse(dto.EquipmentTypeId))
            .ToExpression();
        var list = _db
            .Queryable<Core.SugarEntities.MetricalData, MetricalGroup, Core.SugarEntities.Specification, MeasureType,
                Turn, Machine>((c, g, s, mt, turn, mac) => new JoinQueryInfos(
                JoinType.Left, c.GroupId == g.Id,
                JoinType.Left, g.SpecificationId == s.Id,
                JoinType.Left, g.MeasureTypeId == mt.Id,
                JoinType.Left, g.TurnId == turn.Id,
                JoinType.Left, g.MachineId == mac.Id
            )).Where(exp).Select((c, g, s, mt, turn, mac) => new
            {
                c.Id,
                c.GroupId,
                specificationId = g.SpecificationId,
                specificationName = s.Name,
                turnName = turn.Name,
                machineName = mac.Name,
                measureTypeName = mt.Name,
                testTime = c.TestTime,
                data = c.Data,
                userData = g.UserData,
                instance = g.Instance,
                equipmentType = g.EquipmentType
            }).ToList();

        var indicators = _iRepo.All().ToList();
        using var package = new ExcelPackage();

        var specificationGroups = list.GroupBy(c => c.specificationId).ToList();
        var specifications = _spRepo.All().Select(c => new {c.Id, c.SingleRules}).ToList();
        foreach (var specificationGroup in specificationGroups)
        {
            var first = specificationGroup.First();
            if (first == null)
                continue;
            var singleRules = specifications.FirstOrDefault(c => c.Id == specificationGroup.Key);
            if (singleRules == null)
                continue;
            var ws = package.Workbook.Worksheets.Add(first.specificationName);

            var rules = JsonConvert.DeserializeObject<List<Rule>>(singleRules.SingleRules);
            var ruleDic = new Dictionary<Rule, Core.Entities.Indicator>();
            foreach (var rule in rules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                if (first.equipmentType == EquipmentType.SingleResistance)
                    indicator.Unit = "mmWG";
                ruleDic.Add(rule, indicator);
            }

            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Column(1).Width = 25;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 25;
            ws.Column(4).Width = 25;
            ws.Column(5).Width = 25;
            ws.Column(6).Width = 25;
            ws.Cells[1, 1].Value = "测量时间";
            ws.Cells[1, 2].Value = "班次";
            ws.Cells[1, 3].Value = "机台";
            ws.Cells[1, 4].Value = "仪器名称";
            ws.Cells[1, 5].Value = "工号";
            ws.Cells[1, 6].Value = "测量类型";
            var col = 7;
            foreach (var ruleItem in ruleDic)
            {
                ws.Column(col).Width = 25;
                ws.Cells[1, col].Value = ruleItem.Value.Name + "(" + ruleItem.Value.Unit + ")";
                col++;
            }

            var row = 2;
            foreach (var item in specificationGroup.OrderBy(c => c.testTime))
            {
                ws.Cells[row, 1].Value = item.testTime.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cells[row, 2].Value = item.turnName;
                ws.Cells[row, 3].Value = item.machineName;
                ws.Cells[row, 4].Value = item.instance;
                ws.Cells[row, 5].Value = item.userData;
                ws.Cells[row, 6].Value = item.measureTypeName;
                var valueCol = 7;
                var obj = JsonConvert.DeserializeObject<JObject>(item.data);
                foreach (var ruleItem in ruleDic)
                {
                    var value = "";
                    if (obj[$"{ruleItem.Key.Id}"] != null) value = obj[$"{ruleItem.Key.Id}"].ToString();

                    if (item.equipmentType == EquipmentType.SingleResistance &&
                        ruleItem.Key.Id == _settings.Resistance)
                    {
                        double.TryParse(value, out var temp);
                        var resistance = ConvertHelper.paToMMWG(temp);
                        resistance = Math.Round(resistance, 0, MidpointRounding.ToPositiveInfinity);
                        ws.Cells[row, valueCol].Value = resistance;
                    }
                    else
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


    public MemoryStream DownloadStatistic(MetricalDataQueryDto dto)
    {
        var exp = Expressionable
            .Create<Core.SugarEntities.MetricalData, MetricalGroup, Core.SugarEntities.Specification, MeasureType,
                Turn, Machine>()
            .AndIF(dto.SpecificationId != null,
                (c, g, s, mt, turn, mac) => g.SpecificationId == int.Parse(dto.SpecificationId))
            .AndIF(dto.BeginTime != null && dto.EndTime != null,
                (c, g, s, mt, turn, mac) => g.BeginTime.Date >= Convert.ToDateTime(dto.BeginTime).Date &&
                                            g.BeginTime.Date <= Convert.ToDateTime(dto.EndTime).Date)
            .AndIF(dto.SpecificationTypeId != null,
                (c, g, s, mt, turn, mac) => s.SpecificationTypeId == int.Parse(dto.SpecificationTypeId))
            .AndIF(dto.TurnId != null, (c, g, s, mt, turn, mac) => g.TurnId == int.Parse(dto.TurnId))
            .AndIF(dto.MeasureTypeId != null,
                (c, g, s, mt, turn, mac) => g.MeasureTypeId == int.Parse(dto.MeasureTypeId))
            .AndIF(dto.MachineModelId != null,
                (c, g, s, mt, turn, mac) => g.MachineId == int.Parse(dto.MachineModelId))
            .AndIF(dto.EquipmentTypeId != null,
                (c, g, s, mt, turn, mac) => g.EquipmentType == (EquipmentType) int.Parse(dto.EquipmentTypeId))
            .ToExpression();
        var list = _db
            .Queryable<Core.SugarEntities.MetricalData, MetricalGroup, Core.SugarEntities.Specification, MeasureType
                , Turn, Machine>((c, g, s, mt, turn, mac) => new JoinQueryInfos(
                JoinType.Left, c.GroupId == g.Id,
                JoinType.Left, g.SpecificationId == s.Id,
                JoinType.Left, g.MeasureTypeId == mt.Id,
                JoinType.Left, g.TurnId == turn.Id,
                JoinType.Left, g.MachineId == mac.Id
            )).Where(exp).Select((c, g, s, mt, turn, mac) => new
            {
                c.Id,
                c.GroupId,
                specificationId = g.SpecificationId,
                specificationName = s.Name,
                turnName = turn.Name,
                machineName = mac.Name,
                measureTypeName = mt.Name,
                testTime = c.TestTime,
                data = c.Data,
                userData = g.UserData,
                instance = g.Instance,
                equipmentType = g.EquipmentType
            }).ToList();

        var specificationGroups = list.GroupBy(c => c.specificationId).ToList();

        var specifications = _spRepo.All().Select(c => new {c.Id, c.SingleRules}).ToList();
        var indicators = _iRepo.All().ToList();
        using var package = new ExcelPackage();
        var statisticItems = new List<string>
            {"平均值", "最大值", "最小值", "SD", "CV", "CPK", "Offs", "上超", "下超", "合格数", "总数", "合格率"};

        foreach (var specificationGroup in specificationGroups)
        {
            var first = specificationGroup.First();
            if (first == null)
                continue;
            var singleRules = specifications.FirstOrDefault(c => c.Id == specificationGroup.Key);
            if (singleRules == null)
                continue;
            var ws = package.Workbook.Worksheets.Add(first.specificationName);

            var rules = JsonConvert.DeserializeObject<List<Rule>>(singleRules.SingleRules);
            var ruleDic = new Dictionary<Rule, Core.Entities.Indicator>();
            foreach (var rule in rules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                if (rule.Id == _settings.Resistance &&
                    first.equipmentType == EquipmentType.SingleResistance)
                {
                    indicator.Unit = "mmWG";
                }

                ruleDic.Add(rule, indicator);
            }

            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Column(1).Width = 25;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 25;
            ws.Column(4).Width = 25;
            ws.Cells[1, 1].Value = "测量时间";
            ws.Cells[1, 1, 2, 1].Merge = true;
            ws.Cells[1, 1, 2, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[1, 2].Value = "班次";
            ws.Cells[1, 2, 2, 2].Merge = true;
            ws.Cells[1, 2, 2, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[1, 3].Value = "机台";
            ws.Cells[1, 3, 2, 3].Merge = true;
            ws.Cells[1, 3, 2, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            ws.Cells[1, 4].Value = "测量类型";
            ws.Cells[1, 4, 2, 4].Merge = true;
            ws.Cells[1, 4, 2, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            var col = 5;
            foreach (var ruleItem in ruleDic)
            {
                var value = ruleItem.Value.Name + "(" + ruleItem.Value.Unit + ")";
                ws.Cells[1, col].Value = value;
                ws.Cells[1, col, 1, col + statisticItems.Count - 1].Merge = true;
                ws.Cells[1, col, 1, col + statisticItems.Count - 1].Style.Border
                    .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                foreach (var item in statisticItems)
                {
                    ws.Column(col).Width = 15;
                    ws.Cells[2, col].Value = item;
                    ws.Cells[2, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    col++;
                }
            }

            var row = 3;
            var groups = specificationGroup.OrderByDescending(c => c.testTime).GroupBy(c => c.GroupId).ToList();
            foreach (var group in groups)
            {
                var item = group.FirstOrDefault();
                if (item == null)
                    continue;
                var dataList = group.Select(c => c.data).ToList();
                var valueCol = 1;
                ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                ws.Cells[row, valueCol++].Value = item.testTime.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                ws.Cells[row, valueCol++].Value = item.turnName;
                ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                ws.Cells[row, valueCol++].Value = item.machineName;
                ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                ws.Cells[row, valueCol++].Value = item.measureTypeName;
                foreach (var ruleItem in ruleDic)
                {
                    var tempList = new List<double>();
                    foreach (var data in dataList)
                    {
                        var obj = JsonConvert.DeserializeObject<JObject>(data);
                        if (obj[$"{ruleItem.Key.Id}"] != null)
                        {
                            var value = obj[$"{ruleItem.Key.Id}"].ToString();
                            if (!string.IsNullOrEmpty(value))
                            {
                                tempList.Add(Convert.ToDouble(value));
                            }
                        }
                    }

                    var statistic = tempList.toStatistic(ruleItem.Key.Standard, ruleItem.Key.Upper,
                        ruleItem.Key.Lower);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.Mean.format(_settings.IndicatorDecimal.Mean);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.Max.format(_settings.IndicatorDecimal.Max);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.Min.format(_settings.IndicatorDecimal.Min);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.Sd.format(_settings.IndicatorDecimal.Sd);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.Cv.format(_settings.IndicatorDecimal.Cv);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.Cpk.format(_settings.IndicatorDecimal.Cpk);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.Offset.format(_settings.IndicatorDecimal.Offs);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.HighCnt;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.LowCnt;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.QualityCount;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.TotalCount;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row, valueCol++].Value = statistic.QualityRate;
                }

                row++;
            }
        }

        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }

    public MemoryStream DownloadStatisticInfo(MetricalDataQueryDto dto)
    {
        var exp = Expressionable
            .Create<Core.SugarEntities.MetricalData, MetricalGroup, Core.SugarEntities.Specification, MeasureType,
                Turn, Machine>()
            .AndIF(dto.SpecificationId != null,
                (c, g, s, mt, turn, mac) => g.SpecificationId == int.Parse(dto.SpecificationId))
            .AndIF(dto.BeginTime != null && dto.EndTime != null,
                (c, g, s, mt, turn, mac) => g.BeginTime.Date >= Convert.ToDateTime(dto.BeginTime).Date &&
                                            g.BeginTime.Date <= Convert.ToDateTime(dto.EndTime).Date)
            .AndIF(dto.SpecificationTypeId != null,
                (c, g, s, mt, turn, mac) => s.SpecificationTypeId == int.Parse(dto.SpecificationTypeId))
            .AndIF(dto.TurnId != null, (c, g, s, mt, turn, mac) => g.TurnId == int.Parse(dto.TurnId))
            .AndIF(dto.MeasureTypeId != null,
                (c, g, s, mt, turn, mac) => g.MeasureTypeId == int.Parse(dto.MeasureTypeId))
            .AndIF(dto.MachineModelId != null,
                (c, g, s, mt, turn, mac) => g.MachineId == int.Parse(dto.MachineModelId))
            .AndIF(dto.EquipmentTypeId != null,
                (c, g, s, mt, turn, mac) => g.EquipmentType == (EquipmentType) int.Parse(dto.EquipmentTypeId))
            .ToExpression();
        var list = _db
            .Queryable<Core.SugarEntities.MetricalData, MetricalGroup, Core.SugarEntities.Specification, MeasureType
                , Turn, Machine>((c, g, s, mt, turn, mac) => new JoinQueryInfos(
                JoinType.Left, c.GroupId == g.Id,
                JoinType.Left, g.SpecificationId == s.Id,
                JoinType.Left, g.MeasureTypeId == mt.Id,
                JoinType.Left, g.TurnId == turn.Id,
                JoinType.Left, g.MachineId == mac.Id
            )).Where(exp).Select((c, g, s, mt, turn, mac) => new
            {
                c.Id,
                c.GroupId,
                specificationId = g.SpecificationId,
                specificationName = s.Name,
                turnName = turn.Name,
                machineName = mac.Name,
                measureTypeName = mt.Name,
                testTime = c.TestTime,
                data = c.Data,
                userData = g.UserData,
                instance = g.Instance,
                equipmentType = g.EquipmentType
            }).ToList();

        var specificationGroups = list.GroupBy(c => c.specificationId).ToList();

        var specifications = _spRepo.All().Select(c => new {c.Id, c.SingleRules}).ToList();
        var indicators = _iRepo.All().ToList();
        using var package = new ExcelPackage();
        var statisticItems = new List<string>
            {"平均值", "最大值", "最小值", "SD", "CV", "CPK", "Offs", "上超", "下超", "合格数", "合格数(优)", "总数", "合格率", "合格率(优)"};
        var sheets = new Dictionary<string, object>();
        foreach (var specificationGroup in specificationGroups)
        {
            var columns = new List<Dictionary<string, object>>();
            var first = specificationGroup.First();
            if (first == null)
                continue;
            var singleRules = specifications.FirstOrDefault(c => c.Id == specificationGroup.Key);
            if (singleRules == null)
                continue;
            var ws = package.Workbook.Worksheets.Add(first.specificationName);

            var rules = JsonConvert.DeserializeObject<List<Rule>>(singleRules.SingleRules);
            var ruleDic = new Dictionary<Rule, Core.Entities.Indicator>();
            foreach (var rule in rules)
            {
                var indicator = indicators.FirstOrDefault(c => c.Id == rule.Id);
                if (rule.Id == _settings.Resistance &&
                    Convert.ToInt32(dto.EquipmentTypeId) == (int) EquipmentType.SingleResistance)
                {
                    indicator.Unit = "mmWG";
                }

                ruleDic.Add(rule, indicator);
            }

            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.WrapText = true;
            var indicatorCount = ruleDic.Count;
            var row = 1;
            var groups = specificationGroup.OrderByDescending(c => c.testTime).GroupBy(c => c.GroupId).ToList();
            foreach (var group in groups)
            {
                var item = group.FirstOrDefault();
                if (item == null)
                    continue;
                var dataList = group.Select(c => c.data).ToList();
                ws.Cells[row, 1, row, indicatorCount + 1].Merge = true;
                ws.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[row++, 1].Value = "测量时间: " + item.testTime.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[row, 1, row, indicatorCount + 1].Merge = true;
                ws.Cells[row++, 1].Value = "班次: " + item.turnName;
                ws.Cells[row, 1, row, indicatorCount + 1].Merge = true;
                ws.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[row++, 1].Value = "机台: " + item.machineName;
                ws.Cells[row, 1, row, indicatorCount + 1].Merge = true;
                ws.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[row++, 1].Value = "测量类型: " + item.measureTypeName;
                ws.Row(row).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                var col = 2;
                row++;
                ws.Cells[row, 1].Value = "序号";
                ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                ws.Column(1).Width = 15;
                foreach (var ruleItem in ruleDic)
                {
                    var value = ruleItem.Value.Name + "(" + ruleItem.Value.Unit + ")";
                    ws.Cells[row, col].Value = value;
                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Column(col).Width = 20;
                    col++;
                }

                row++;
                var valueCol = 2;
                var index = 1;
                var originDataDic = new Dictionary<int, List<double>>();
                foreach (var data in dataList)
                {
                    ws.Cells[row, 1].Value = index;
                    ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    var obj = JsonConvert.DeserializeObject<JObject>(data);
                    foreach (var ruleItem in ruleDic)
                    {
                        var value = "";
                        if (obj[$"{ruleItem.Key.Id}"] != null)
                            value = obj[$"{ruleItem.Key.Id}"].ToString();
                        double doubleValue = 0;
                        if (!string.IsNullOrEmpty(value))
                        {
                            doubleValue = Convert.ToDouble(value);
                            if (originDataDic.TryGetValue(ruleItem.Key.Id, out var valueList))
                                valueList.Add(doubleValue);
                            else
                                originDataDic.Add(ruleItem.Key.Id, new List<double> {doubleValue});
                        }

                        ws.Cells[row, valueCol].Value = doubleValue == 0 ? "" : doubleValue;
                        ws.Cells[row, valueCol].Style.Numberformat.Format = doubleValue.getDecimalCountStr();
                        var standard = Convert.ToDouble(ruleItem.Key.Standard);
                        var high = Convert.ToDouble(ruleItem.Key.Upper);
                        var low = Convert.ToDouble(ruleItem.Key.Lower);
                        if (item.equipmentType == EquipmentType.Rt)
                        {
                            high = standard + high;
                            low = standard - low;
                        }

                        if (doubleValue > 0)
                        {
                            if (doubleValue > high)
                                ws.Cells[row, valueCol].Style.Font.Color.SetColor(Color.Red);
                            else if (doubleValue < low)
                                ws.Cells[row, valueCol].Style.Font.Color.SetColor(Color.Blue);
                        }

                        ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        valueCol++;
                    }

                    valueCol = 2;
                    index++;
                    row++;
                }

                col = 2;
                ws.Cells[row, 1].Value = "";
                ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                foreach (var ruleItem in ruleDic)
                {
                    var value = ruleItem.Value.Name + "(" + ruleItem.Value.Unit + ")";
                    ws.Cells[row, col].Value = value;
                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    col++;
                }

                row++;
                var beginRow = row;
                foreach (var staItem in statisticItems)
                {
                    ws.Cells[row, 1].Value = staItem;
                    ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    row++;
                }

                row = beginRow;
                valueCol = 2;
                foreach (var ruleItem in ruleDic)
                {
                    if (!originDataDic.ContainsKey(ruleItem.Key.Id))
                    {
                        var tempRow = row;
                        foreach (var st in statisticItems)
                        {
                            ws.Cells[tempRow, valueCol].Style.Border
                                .BorderAround(ExcelBorderStyle.Thin, Color.Black);
                            tempRow++;
                        }

                        valueCol++;
                        continue;
                    }

                    var high = Convert.ToDouble(ruleItem.Key.Upper);
                    var low = Convert.ToDouble(ruleItem.Key.Lower);
                    var standard = Convert.ToDouble(ruleItem.Key.Standard);
                    if (item.equipmentType == EquipmentType.Rt)
                    {
                        high = standard + high;
                        low = standard - low;
                    }

                    var originDataList = originDataDic[ruleItem.Key.Id];
                    var statistic =
                        originDataList.toStatistic(ruleItem.Key, item.equipmentType != EquipmentType.Rt);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.Mean.format(_settings.IndicatorDecimal.Mean);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.Max.format(_settings.IndicatorDecimal.Max);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.Min.format(_settings.IndicatorDecimal.Min);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.Sd.format(_settings.IndicatorDecimal.Sd);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.Cv.format(_settings.IndicatorDecimal.Cv);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.Cpk.format(_settings.IndicatorDecimal.Cpk);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.Offset.format(_settings.IndicatorDecimal.Offs);
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.HighCnt;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.LowCnt;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.QualityCount;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.QualityQualityCount;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.TotalCount;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.QualityRate;
                    ws.Cells[row, valueCol].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    ws.Cells[row++, valueCol].Value = statistic.QualityQualityRate;

                    valueCol++;
                    row = beginRow;
                }

                row += statisticItems.Count + 2;
            }
        }

        // MiniExcel.SaveAs("D:\\test.xlsx", sheets);
        var file = new MemoryStream();
        package.SaveAs(file);

        file.Seek(0, SeekOrigin.Begin);
        return file;
    }

    public IEnumerable<BaseOptionDto> GetMeasureDataBySpecificationIdAndMeasureTypeId(int specificationId,
        int measureTypeId)
    {
        var exp = Expressionable.Create<MetricalGroup>()
            .AndIF(true, c => c.SpecificationId == specificationId)
            .AndIF(true, c => c.MeasureTypeId == measureTypeId)
            .ToExpression();
        var g = _db.Queryable<MetricalGroup>()
            .LeftJoin<Core.SugarEntities.Specification>((c, s) => c.SpecificationId == s.Id)
            .Where(exp).OrderByDescending(c => c.BeginTime)
            .Select((c, s) => new BaseOptionDto()
            {
                Value = c.Id,
                Text = $"{s.Name}( {c.BeginTime:yyyy-MM-dd HH:mm:ss} )"
            }).ToList();

        return g;
    }

    private JObject getSpecificationIndicators(int id)
    {
        var group = _gRepo.Get(id);
        var specification = _spRepo.Get(group.SpecificationId);
        var label = new JObject {{"label", "原始数据"}, {"type", "table-editor"}};
        var columns = new JArray();
        var rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);

        var idObj = new JObject {{"prop", "id"}, {"label", "ID"}, {"width", "150"}};
        columns.Add(idObj);
        var testTime = new JObject {{"prop", "testTime"}, {"label", "测量时间"}, {"width", "300"}};
        var testTimeContent = new JObject {{"type", "el-date-picker"}};
        var testTimeAttrs = new JObject
        {
            {"type", "datetime"}, {"value-format", "yyyy-MM-dd HH:mm:ss"},
            {"default-value", group.BeginTime.Date},
            {"default-time", $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}"}
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
            var column = new JObject {{"prop", rule.Id.ToString()}, {"label", indicatorName}, {"width", 150}};
            var content = new JObject {{"type", "el-input"}};
            var attrs = new JObject {{"type", "number"}, {"step", "0.0000001"}};
            content.Add("attrs", attrs);
            column.Add("content", content);
            columns.Add(column);
        }

        var tableAttrs = new JObject {{"ref", "dataTable"}, {"height", "500"}};
        var parentAttrs = new JObject {{"tableAttrs", tableAttrs}, {"columns", columns}};
        label.Add("attrs", parentAttrs);
        var dataObj = new JObject {{"data", label}};
        var specificationObj = new JObject {{"id", specification.Id}, {"desc", dataObj}};

        return specificationObj;
    }

    public async Task<IEnumerable<MetricalDataInfoDto>> GetMetricalDataInfoAsync(int type)
    {
        var dataRepository = base.ChangeRepository<SugarRepository<Core.SugarEntities.MetricalData>>();

        var list = new List<MetricalDataInfoDto>();
        var begin = DateTime.Now.Date;
        var end = DateTime.Now.Date;

        switch (type)
        {
            case 1:
                begin = end.AddDays(-7);
                break;
            case 2:
                begin = end.AddDays(-30);
                break;
            case 3:
                begin = end.AddYears(-1);
                break;
        }

        var filter = Expressionable.Create<Core.SugarEntities.MetricalData>()
            .And(c => c.Group.BeginTime.Date >= begin && c.Group.EndTime.Date <= end)
            .ToExpression();
        var groupList = await base.Context.Queryable<MetricalGroup>()
            .Includes(c => c.DataList)
            .Where(c => c.BeginTime.Date >= begin && c.EndTime.Date <= end)
            .ToListAsync();

        if (type == 1)
        {
            var temp = groupList.GroupBy(c => c.BeginTime.Date.Hour).ToList();
            foreach (var group in temp)
            {
                var item = new MetricalDataInfoDto();
                var groupTotal = group.Count();
                var dataTotal = group.Sum(c => c.DataList.Count);
                item.Name = group.Key.ToString();
                item.GroupTotal = groupTotal;
                item.DataTotal = dataTotal;
                list.Add(item);
            }
        }
        else if (type == 2)
        {
            var temp = groupList.GroupBy(c => c.BeginTime.Date.Day).ToList();
            foreach (var group in temp)
            {
                var item = new MetricalDataInfoDto();
                var groupTotal = group.Count();
                var dataTotal = group.Sum(c => c.DataList.Count);
                item.Name = group.Key.ToString();
                item.GroupTotal = groupTotal;
                item.DataTotal = dataTotal;
                list.Add(item);
            }
        }
        else if (type == 3)
        {
            var temp = groupList.GroupBy(c => c.BeginTime.Month).ToList();
            foreach (var group in temp)
            {
                var item = new MetricalDataInfoDto();
                var groupTotal = group.Count();
                var dataTotal = group.Sum(c => c.DataList.Count);
                item.Name = group.Key.ToString();
                item.GroupTotal = groupTotal;
                item.DataTotal = dataTotal;
                list.Add(item);
            }
        }

        return list.OrderBy(c => int.Parse(c.Name)).ToList();
    }

    public async Task<IEnumerable<MetricalDataInfoDto>> GetManualMetricalDataInfoAsync(int type, string workShopName)
    {
        var dataRepository = base.ChangeRepository<SugarRepository<Core.SugarEntities.MetricalData>>();

        var list = new List<MetricalDataInfoDto>();
        var begin = DateTime.Now.Date;
        var end = DateTime.Now.Date;

        switch (type)
        {
            case 1:
                break;
            case 2:
                begin = end.AddDays(-7);
                break;
            case 3:
                begin = end.AddMonths(-1);
                break;
        }

        var filter = Expressionable.Create<Core.SugarEntities.MetricalData>()
            .And(c => c.Group.BeginTime.Date >= begin && c.Group.EndTime.Date <= end)
            .And(c => c.Group.EquipmentType == EquipmentType.SingleResistance)
            .ToExpression();
        var groupList = await base.Context.Queryable<MetricalGroup>()
            .Includes(c => c.DataList)
            .Where(c => c.BeginTime.Date >= begin && c.EndTime.Date <= end &&
                        c.EquipmentType == EquipmentType.SingleResistance &&
                        c.Instance.Contains(workShopName))
            .ToListAsync();

        if (type == 1)
        {
            var temp = groupList.GroupBy(c => c.BeginTime.Date.Hour).ToList();
            foreach (var group in temp)
            {
                var item = new MetricalDataInfoDto();
                var groupTotal = group.Count();
                var dataTotal = group.Sum(c => c.DataList.Count);
                item.Name = group.Key.ToString();
                item.GroupTotal = groupTotal;
                item.DataTotal = dataTotal;
                list.Add(item);
            }
        }
        else if (type == 2)
        {
            var temp = groupList.GroupBy(c => c.BeginTime.Date.Day).ToList();
            foreach (var group in temp)
            {
                var item = new MetricalDataInfoDto();
                var groupTotal = group.Count();
                var dataTotal = group.Sum(c => c.DataList.Count);
                item.Name = group.Key.ToString();
                item.GroupTotal = groupTotal;
                item.DataTotal = dataTotal;
                list.Add(item);
            }
        }
        else if (type == 3)
        {
            var temp = groupList.GroupBy(c => c.BeginTime.Day).ToList();
            foreach (var group in temp)
            {
                var item = new MetricalDataInfoDto();
                var groupTotal = group.Count();
                var dataTotal = group.Sum(c => c.DataList.Count);
                item.Name = group.Key.ToString();
                item.GroupTotal = groupTotal;
                item.DataTotal = dataTotal;
                list.Add(item);
            }
        }

        return list.OrderBy(c => int.Parse(c.Name)).ToList();
    }

    private List<DashboardDto.ManualCheckerInfo> getManualCheckerInfos(string workshopName, DateTime begin,
        DateTime end, List<int> specificationIds, List<int> turnIds)
    {
        var query = base.Context.Queryable<MetricalGroup>()
            .Includes(c => c.DataList)
            .Where(c => c.BeginTime.Date >= begin &&
                        c.EndTime.Date <= end && !string.IsNullOrEmpty(c.UserData));
        if (!string.IsNullOrEmpty(workshopName))
        {
            query = query.Where(c => c.Instance.Contains(workshopName));
        }
        if (specificationIds.Count > 0)
        {
            query = query.Where(c => specificationIds.Contains(c.SpecificationId));
        }

        if (turnIds.Count > 0)
        {
            query = query.Where(c => turnIds.Contains(c.TurnId));
        }
        var groups = query.ToList().GroupBy(c => c.UserData).ToList();
        var specifications = _spRepo.All().ToList();
        var list = new List<DashboardDto.ManualCheckerInfo>();
        foreach (var group in groups)
        {
            var result = new DashboardDto.ManualCheckerInfo()
            {
                No = group.Key
            };
            foreach (var g in group)
            {
                var specification = specifications.FirstOrDefault(c => c.Id == g.SpecificationId);
                if (specification == null) continue;
                var rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
                var rule = rules.FirstOrDefault(c => c.Id == _settings.Resistance);
                if (rule == null) continue;
                var valueList = new List<double>();
                foreach (var data in g.DataList)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(data.Data);
                    if (obj == null) continue;
                    var value = obj[_settings.Resistance.ToString()];
                    if (value != null)
                    {
                        if (!string.IsNullOrEmpty(value.ToString()))
                            valueList.Add(double.Parse(value.ToString()));
                    }
                }

                var statistic = valueList.toStatistic(rule.Standard, rule.Upper, rule.Lower);
                result.Count += statistic.TotalCount;
                result.LessCount += statistic.LowCnt;
                result.MoreCount += statistic.HighCnt;
                result.QualityCount += statistic.QualityCount;
                result.GoodCount += statistic.QualityQualityCount;
            }

            result.QualifiedRate =
                Math.Round((Convert.ToDouble(result.QualityCount) / result.Count) * 100, 2);
            result.GoodRate = Math.Round((Convert.ToDouble(result.GoodCount) / result.Count) * 100, 2);
            

            list.Add(result);
        }

        return list;
    }
    public List<DashboardDto.ManualCheckerInfo> GetManualCheckerInfos(string workshopName)
    {
        var begin = DateTime.Now.Date;
        var end = DateTime.Now.Date;
        var query = base.Context.Queryable<MetricalGroup>()
            .Includes(c => c.DataList)
            .Where(c => c.Instance.Contains(workshopName)).ToList();
        var groups = query.GroupBy(c => c.UserData).ToList();
        var specifications = _spRepo.All().ToList();
        var list = new List<DashboardDto.ManualCheckerInfo>();
        foreach (var group in groups)
        {
            var result = new DashboardDto.ManualCheckerInfo()
            {
                No = group.Key
            };
            foreach (var g in group)
            {
                var specification = specifications.FirstOrDefault(c => c.Id == g.SpecificationId);
                if (specification == null) continue;
                var rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
                var rule = rules.FirstOrDefault(c => c.Id == _settings.Resistance);
                if (rule == null) continue;
                var valueList = new List<double>();
                foreach (var data in g.DataList)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(data.Data);
                    if (obj == null) continue;
                    var value = obj[_settings.Resistance.ToString()];
                    if (value != null)
                    {
                        if (!string.IsNullOrEmpty(value.ToString()))
                            valueList.Add(double.Parse(value.ToString()));
                    }
                }

                var statistic = valueList.toStatistic(rule.Standard, rule.Upper, rule.Lower);
                result.LessCount += statistic.LowCnt;
                result.MoreCount += statistic.HighCnt;
                result.QualityCount += statistic.QualityCount;
            }

            list.Add(result);
        }

        return list;
    }

    // <summary>
    /// 获取手工车间
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<MaterialDataHandicraftWorkshop>> GetHandicraftWorkshopAsync()
    {
        var list = new List<MaterialDataHandicraftWorkshop>()
        {
            new MaterialDataHandicraftWorkshop()
            {
                ID = Guid.NewGuid().ToString(),
                Name = "甲",
                letters = "J"
            },
            new MaterialDataHandicraftWorkshop()
            {
                ID = Guid.NewGuid().ToString(),
                Name = "乙",
                letters = "Y"
            },
            new MaterialDataHandicraftWorkshop()
            {
                ID = Guid.NewGuid().ToString(),
                Name = "丙",
                letters = "B"
            },
        };
        return list;
    }

    public IEnumerable<MetricalDataTableDto> GetHandicraftWorkshopMatrialData(string WorkShopLetter,
        int PageSize, int PageNum, out int total)
    {
        total = 0;
        var exp = Expressionable.Create<MetricalGroup>()
            .AndIF(WorkShopLetter != null, c => SqlFunc.StartsWith(c.Instance, WorkShopLetter))
            .ToExpression();
        var list = _db.Queryable<MetricalGroup>()
            .LeftJoin<Core.SugarEntities.Specification>((c, s) => c.SpecificationId == s.Id)
            .LeftJoin<Team>((c, s, team) => c.TeamId == team.Id)
            .LeftJoin<Turn>((c, s, team, turn) => c.TurnId == turn.Id)
            .LeftJoin<Machine>((c, s, team, turn, mac) => c.MachineId == mac.Id)
            .LeftJoin<MeasureType>((c, s, team, turn, mac, mt) => c.MeasureTypeId == mt.Id)
            .Where(exp)
            .OrderByDescending(c => c.BeginTime)
            .Select((c, s, team, turn, mac, mt) => new MetricalDataTableDto()
            {
                Id = c.Id,
                SpecificationId = c.SpecificationId,
                SpecificationName = s.Name,
                SpecificationTypeId = c.Specification.SpecificationTypeId,
                TeamId = c.TeamId,
                TeamName = team.Name,
                TurnName = turn.Name,
                MachineId = c.MachineId,
                MachineName = mac.Name,
                MeasureTypeName = mt.Name,
                BeginTime = c.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = c.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductionTime = c.ProductionTime == null
                    ? ""
                    : Convert.ToDateTime(c.ProductionTime).ToString("yyyy-MM-dd"),
                TurnId = c.TurnId,
                MeasureTypeId = c.MeasureTypeId,
                DeliverTime = c.DeliverTime == null ? "" : Convert.ToDateTime(c.DeliverTime).ToString("yyyy-MM-dd"),
                OrderNo = c.OrderNo ?? "",
                Instance = c.Instance,
                UserName = c.UserName,
                EquipmentType = c.EquipmentType,
                UserData = c.UserData
            })
            .ToPageList(PageNum, PageSize, ref total);

        foreach (var item in list)
        {
            item.EquipmentTypeName = item.EquipmentType.toDescription();
            if (item.EquipmentType == EquipmentType.SingleResistance)
            {
                item.UserName = item.UserData;
            }
        }

        return list;
    }

    public ManualDataPushDto GetManualMetricalDataStatistic(string workShopLetter)
    {
        var dto = new ManualDataPushDto();
        var begin = DateTime.Now.AddMonths(-6);
        var end = DateTime.Now;
        var groups = _db.Queryable<MetricalGroup>().Includes(c=>c.DataList).Where(c =>
            c.Instance.Contains(workShopLetter) &&
            c.BeginTime.Date >= begin &&
            c.EndTime.Date <= end).ToList();
        var specificationGroups = groups.GroupBy(c => c.SpecificationId).ToList();
        var specifications = _spRepo.All().ToList();
        var tableList = new List<ManualDataPushDto.ManualDataPushTable>();
        var pieChartInfos = new List<ManualDataPushDto.PieChartInfo>();
        foreach (var spGroup in specificationGroups)
        {
            var specification = specifications.FirstOrDefault(c => c.Id == spGroup.Key);
            if (specification == null) continue;
            var rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
            var rule = rules.FirstOrDefault(c => c.Id == _settings.Resistance);
            if (rule == null) continue;
            var valueList = new List<double>();
            foreach (var group in spGroup)
            {
                foreach (var data in group.DataList)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(data.Data);
                    var value = obj?[_settings.Resistance.ToString()];
                    if (value == null) continue;
                    if (!string.IsNullOrEmpty(value.ToString()))
                        valueList.Add(double.Parse(value.ToString()));
                }
            }

            var statistic = valueList.toStatistic(rule, true);
            var tableInfo = new ManualDataPushDto.ManualDataPushTable()
            {
                SpecificationName = specification.Name,
                Mean = Math.Round(statistic.Mean, 3),
                Total = statistic.TotalCount,
                Quality = statistic.QualityCount,
                QualityInfo = statistic.QualityQualityRate,
                Rate = statistic.QualityRate
            };
            tableList.Add(tableInfo);
            pieChartInfos.Add(new ManualDataPushDto.PieChartInfo()
            {
                Name = specification.Name,
                Value = statistic.QualityCount,
                ResistanceMean = statistic.Mean.ToString("#.###"),
                QualifiedRate = statistic.QualityRate,
                GoodQualifiedRate = statistic.QualityQualityRate
            });
        }

        dto.TableInfo = tableList;
        dto.PieChartInfos = pieChartInfos;

        return dto;
    }

    public IEnumerable<BaseOptionDto> GetNewestGroupIdsByMachine(NewestGroupIdsQueryDto dto)
    {
        var groups = _db.Queryable<MetricalGroup>().Includes(c=>c.Machine).Where(c=>c.BeginTime.Date >= DateTime.Now.AddMonths(-6) &&
                                                             c.EndTime.Date <= DateTime.Now);

        if (dto.IsMachine)
        {
            var macLs = dto.Machines.Select(int.Parse).ToList();
            groups = groups.Where(c => macLs.Contains(c.MachineId));
        }
        else
        {
            groups = groups.Where(c => dto.Machines.Contains(c.Instance.Substring(0, 1)));
        }

        var orderedGroups = groups.OrderByDescending(c => c.BeginTime).ToList();
        List<BaseOptionDto> ids = new();
        if (dto.IsMachine)
        {
            var machineGroups = orderedGroups.GroupBy(c => c.MachineId).ToList();
            foreach (var gs in machineGroups)
            {
                ids.Add(new BaseOptionDto() { Text = gs.First().Machine.Name, Value = gs.First().Id});
            }
        }
        else
        {
            var instanceGroups = orderedGroups.GroupBy(c => c.Instance[..1]).ToList();
            foreach (var gs in instanceGroups)
            {
                ids.Add(new BaseOptionDto() {Text = gs.First().Instance, Value = gs.First().Id});
            }
        }

        return ids;
    }

    public DashboardDto.ManualSummaryInfoDto GetManualSummaryInfo(ManualQueryInfoDto dto)
    {
        var result = new ManualSummaryInfoDto();
        var begin = DateTime.Now.AddMonths(-6);
        var end = DateTime.Now;
        if (!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End))
        {
            begin = Convert.ToDateTime(dto.Begin);
            end = Convert.ToDateTime(dto.End);
        }

        var groups = _db.Queryable<MetricalGroup>().Includes(c => c.DataList).Where(c =>
            c.BeginTime.Date >= begin &&
            c.EndTime.Date <= end &&
            c.EquipmentType == EquipmentType.SingleResistance);

        if (!string.IsNullOrEmpty(dto.WorkShop))
        {
            groups = groups.Where(c => c.Instance.Contains(dto.WorkShop));
        }

        if (dto.SpecificationId.Count > 0)
        {
            groups = groups.Where(c => dto.SpecificationId.Contains(c.SpecificationId));
        }

        if (dto.TurnIds.Count > 0)
        {
            groups = groups.Where(c => dto.TurnIds.Contains(c.TurnId));
        }

        var tempList = groups.ToList();
        var specificationGroups = tempList.GroupBy(c => c.SpecificationId).ToList();
        var specifications = _spRepo.All().ToList();
        var tableList = new List<ManualDataPushDto.ManualDataPushTable>();
        var pieChartInfos = new List<ManualDataPushDto.PieChartInfo>();
        foreach (var spGroup in specificationGroups)
        {
            var specification = specifications.FirstOrDefault(c => c.Id == spGroup.Key);
            if (specification == null) continue;
            var rules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
            var rule = rules.FirstOrDefault(c => c.Id == _settings.Resistance);
            if (rule == null) continue;
            var valueList = new List<double>();
            foreach (var group in spGroup)
            {
                foreach (var data in group.DataList)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(data.Data);
                    var value = obj?[_settings.Resistance.ToString()];
                    if (value == null) continue;
                    if (!string.IsNullOrEmpty(value.ToString()))
                        valueList.Add(double.Parse(value.ToString()));
                }
            }

            var statistic = valueList.toStatistic(rule, true);
            var tableInfo = new ManualDataPushDto.ManualDataPushTable()
            {
                SpecificationName = specification.Name,
                Mean = Math.Round(statistic.Mean, 3),
                Max = statistic.Max,
                Min = statistic.Min,
                Sd = Math.Round(statistic.Sd, 3),
                Cpk = Math.Round(statistic.Cpk, 3),
                Offset = Math.Round(statistic.Offset, 3),
                Total = statistic.TotalCount,
                Quality = statistic.QualityCount,
                QualityInfo = statistic.QualityQualityRate,
                Rate = statistic.QualityRate
            };
            tableList.Add(tableInfo);
            pieChartInfos.Add(new ManualDataPushDto.PieChartInfo()
            {
                Name = specification.Name,
                Value = statistic.QualityCount,
                ResistanceMean = statistic.Mean.ToString("#.###"),
                QualifiedRate = statistic.QualityRate,
                GoodQualifiedRate = statistic.QualityQualityRate
            });
        }

        result.TableInfo = tableList;
        result.PieChartInfo = pieChartInfos;
        result.CheckerInfo = getManualCheckerInfos(dto.WorkShop, begin, end, dto.SpecificationId, dto.TurnIds);

        return result;
    }

    public IEnumerable<BaseOptionDto> GetSpecificationsByTeamIds(GetSpecificationByTurnsQueryDto dto)
    {
        var bDate = DateTime.Now.AddMonths(-6);
        var eDate = DateTime.Now;
        var query = _db.Queryable<MetricalGroup>();
        if (!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End))
        {
            bDate = Convert.ToDateTime(dto.Begin);
            eDate = Convert.ToDateTime(dto.End);
        }

        query = query.Where(c => c.BeginTime.Date >= bDate.Date &&
                                 c.EndTime.Date <= eDate.Date);

        if (dto.TurnIds.Count > 0)
        {
            query = query.Where(c => dto.TurnIds.Contains(c.TurnId));
        }

        if (dto.IsManual)
        {
            query = query.Where(c => c.EquipmentType == EquipmentType.SingleResistance);
        }

        var specificationIds = query.Select(c => c.SpecificationId).Distinct().ToList();
        var specifications = _db.Queryable<Core.SugarEntities.Specification>()
            .Where(c => specificationIds.Contains(c.Id))
            .Select(c => new BaseOptionDto()
            {
                Value = c.Id, Text = c.Name
            }).ToList();
        return specifications;
    }
}