using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.Specification;
using Core.Dtos.Statistics;
using Core.Entities;
using Core.Enums;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services.Statistics.Impl;

public class FilterStandardDeviationStatisticService : IFilterStandardDeviationStatisticService
{
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _spRepo;

    public FilterStandardDeviationStatisticService(IRepository<Group> gRepo,
        IRepository<Data> dRepo,
        IRepository<Core.Entities.Specification> spRepo,
        IOptionsSnapshot<Settings> settings)
    {
        _gRepo = gRepo;
        _dRepo = dRepo;
        _spRepo = spRepo;
        _settings = settings.Value;
    }

    public FilterStandardDeviationInfoDto Search(StatisticQueryInfoDto dto)
    {
        var data = getData(dto);
        return data;
    }

    private FilterStandardDeviationInfoDto getData(StatisticQueryInfoDto dto)
    {
        var result = new FilterStandardDeviationInfoDto();

        var filterSpecifications = _spRepo.All().Where(c => c.SpecificationTypeId == _settings.FilterTypeId).ToList();
        var filterSpecificationIds = filterSpecifications.Select(c => c.Id).ToList();
        // result.SpecificationName = specification.Name;
        // var singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules);
        var groups = _gRepo.All().AsNoTracking().Where(c =>
            filterSpecificationIds.Contains(c.SpecificationId) && c.MeasureTypeId == _settings.ProductionTypeId);
        if (!string.IsNullOrEmpty(dto.SpecificationId))
        {
            var specificationIds = new List<int>();
            foreach (var item in dto.SpecificationId.Split(",", StringSplitOptions.RemoveEmptyEntries))
                specificationIds.Add(int.Parse(item));
            groups = groups.Where(c => specificationIds.Contains(c.SpecificationId));
        }

        if (!string.IsNullOrEmpty(dto.TurnId))
        {
            var turnId = int.Parse(dto.TurnId);
            groups = groups.Where(c => c.TurnId == turnId);
        }

        if (!string.IsNullOrEmpty(dto.MachineId))
        {
            var machineId = int.Parse(dto.MachineId);
            groups = groups.Where(c => c.MachineModelId == machineId);
        }

        if (!string.IsNullOrEmpty(dto.BeginDate) && !string.IsNullOrEmpty(dto.EndDate))
        {
            var begin = Convert.ToDateTime(dto.BeginDate);
            var end = Convert.ToDateTime(dto.EndDate);
            groups = groups.Where(c => c.BeginTime.Date >= begin &&
                                       c.EndTime.Date <= end);
        }

        if (!string.IsNullOrEmpty(dto.ExcludeDate))
        {
            var excludeDates = new List<DateTime>();
            foreach (var s in dto.ExcludeDate.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                var date = Convert.ToDateTime(s);
                excludeDates.Add(date.Date);
            }

            groups = groups.Where(c => !excludeDates.Contains(c.BeginTime.Date));
        }

        var weight = new List<FilterStandardDeviationInfoDto.SpecificationStatisticInfo>();
        var circle = new List<FilterStandardDeviationInfoDto.SpecificationStatisticInfo>();
        var oval = new List<FilterStandardDeviationInfoDto.SpecificationStatisticInfo>();
        var length = new List<FilterStandardDeviationInfoDto.SpecificationStatisticInfo>();
        var resistance = new List<FilterStandardDeviationInfoDto.SpecificationStatisticInfo>();
        var hardness = new List<FilterStandardDeviationInfoDto.SpecificationStatisticInfo>();
        var groupIds = groups.Select(c => c.Id).Distinct().ToList();
        var groupBeginDate = groups.Select(c => c.BeginTime.Date.ToString("yyyy-MM-dd")).Distinct().ToList();
        var specificationGroups = groups.ToList().GroupBy(c => c.SpecificationId).ToList();
        foreach (var specificationGroup in specificationGroups)
        {
            var specification = filterSpecifications.FirstOrDefault(c => c.Id == specificationGroup.Key);
            if (specification == null) continue;
            var singleRules = JsonConvert.DeserializeObject<List<Rule>>(specification.SingleRules).ToList();
            var weightList = new List<string[]>();
            var circleList = new List<string[]>();
            var ovalList = new List<string[]>();
            var lengthList = new List<string[]>();
            var resistanceList = new List<string[]>();
            var hardnessList = new List<string[]>();
            foreach (var g in specificationGroup)
            {
                var dbData = _dRepo.All().Where(c => c.GroupId == g.Id).ToList();
                var measureData = getFilterMeasureDataList(dbData);
                if (measureData.WeightList.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Weight);
                    if (rule == null)
                        break;
                    var standard = double.Parse(rule.Standard);
                    var upper = double.Parse(rule.Upper);
                    var lower = double.Parse(rule.Lower);
                    var sta = measureData.WeightList.toStatistic(standard, upper, lower);
                    var value = dto.StatisticType switch
                    {
                        CustomerEnum.FilterStatisticPlotType.Sd => sta.Sd,
                        CustomerEnum.FilterStatisticPlotType.Cpk => sta.Cpk,
                        CustomerEnum.FilterStatisticPlotType.Cv => sta.Cv,
                        CustomerEnum.FilterStatisticPlotType.Offs => sta.Offset,
                        CustomerEnum.FilterStatisticPlotType.绝偏 => sta.Mean - standard,
                        _ => 0
                    };
                    weightList.Add(new[] { g.BeginTime.Date.ToString("yyyy-MM-dd"), value.ToString("F3") });
                }

                if (measureData.CircleList.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Circle);
                    if (rule == null)
                        break;
                    var standard = double.Parse(rule.Standard);
                    var upper = double.Parse(rule.Upper);
                    var lower = double.Parse(rule.Lower);
                    var sta = measureData.CircleList.toStatistic(standard, upper, lower);
                    var value = dto.StatisticType switch
                    {
                        CustomerEnum.FilterStatisticPlotType.Sd => sta.Sd,
                        CustomerEnum.FilterStatisticPlotType.Cpk => sta.Cpk,
                        CustomerEnum.FilterStatisticPlotType.Cv => sta.Cv,
                        CustomerEnum.FilterStatisticPlotType.Offs => sta.Offset,
                        CustomerEnum.FilterStatisticPlotType.绝偏 => sta.Mean - standard,
                        _ => 0
                    };
                    circleList.Add(new[] { g.BeginTime.Date.ToString("yyyy-MM-dd"), value.ToString("F3") });
                }

                if (measureData.OvalList.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Oval);
                    if (rule == null)
                        break;
                    var standard = double.Parse(rule.Standard);
                    var upper = double.Parse(rule.Upper);
                    var lower = double.Parse(rule.Lower);
                    var sta = measureData.OvalList.toStatistic(standard, upper, lower);
                    var value = dto.StatisticType switch
                    {
                        CustomerEnum.FilterStatisticPlotType.Sd => sta.Sd,
                        CustomerEnum.FilterStatisticPlotType.Cpk => sta.Cpk,
                        CustomerEnum.FilterStatisticPlotType.Cv => sta.Cv,
                        CustomerEnum.FilterStatisticPlotType.Offs => sta.Offset,
                        CustomerEnum.FilterStatisticPlotType.绝偏 => sta.Mean - standard,
                        _ => 0
                    };
                    ovalList.Add(new[] { g.BeginTime.Date.ToString("yyyy-MM-dd"), value.ToString("F3") });
                }

                if (measureData.LengthList.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Length);
                    if (rule == null)
                        break;
                    var standard = double.Parse(rule.Standard);
                    var upper = double.Parse(rule.Upper);
                    var lower = double.Parse(rule.Lower);
                    var sta = measureData.LengthList.toStatistic(standard, upper, lower);
                    var value = dto.StatisticType switch
                    {
                        CustomerEnum.FilterStatisticPlotType.Sd => sta.Sd,
                        CustomerEnum.FilterStatisticPlotType.Cpk => sta.Cpk,
                        CustomerEnum.FilterStatisticPlotType.Cv => sta.Cv,
                        CustomerEnum.FilterStatisticPlotType.Offs => sta.Offset,
                        CustomerEnum.FilterStatisticPlotType.绝偏 => sta.Mean - standard,
                        _ => 0
                    };
                    lengthList.Add(new[] { g.BeginTime.Date.ToString("yyyy-MM-dd"), value.ToString("F3") });
                }

                if (measureData.ResistanceList.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Resistance);
                    if (rule == null)
                        break;
                    var standard = double.Parse(rule.Standard);
                    var upper = double.Parse(rule.Upper);
                    var lower = double.Parse(rule.Lower);
                    var sta = measureData.ResistanceList.toStatistic(standard, upper, lower);
                    var value = dto.StatisticType switch
                    {
                        CustomerEnum.FilterStatisticPlotType.Sd => sta.Sd,
                        CustomerEnum.FilterStatisticPlotType.Cpk => sta.Cpk,
                        CustomerEnum.FilterStatisticPlotType.Cv => sta.Cv,
                        CustomerEnum.FilterStatisticPlotType.Offs => sta.Offset,
                        CustomerEnum.FilterStatisticPlotType.绝偏 => sta.Mean - standard,
                        _ => 0
                    };
                    resistanceList.Add(new[] { g.BeginTime.Date.ToString("yyyy-MM-dd"), value.ToString("F3") });
                }

                if (measureData.HardnessList.Count > 0)
                {
                    var rule = singleRules.FirstOrDefault(c => c.Id == _settings.Hardness);
                    if (rule == null)
                        break;
                    var standard = double.Parse(rule.Standard);
                    var upper = double.Parse(rule.Upper);
                    var lower = double.Parse(rule.Lower);
                    var sta = measureData.HardnessList.toStatistic(standard, upper, lower);
                    var value = dto.StatisticType switch
                    {
                        CustomerEnum.FilterStatisticPlotType.Sd => sta.Sd,
                        CustomerEnum.FilterStatisticPlotType.Cpk => sta.Cpk,
                        CustomerEnum.FilterStatisticPlotType.Cv => sta.Cv,
                        CustomerEnum.FilterStatisticPlotType.Offs => sta.Offset,
                        CustomerEnum.FilterStatisticPlotType.绝偏 => sta.Mean - standard,
                        _ => 0
                    };
                    hardnessList.Add(new[] { g.BeginTime.Date.ToString("yyyy-MM-dd"), value.ToString("F3") });
                }
            }

            weight.Add(new FilterStandardDeviationInfoDto.SpecificationStatisticInfo
            {
                SpecificationName = specification.Name,
                data = weightList
            });
            circle.Add(new FilterStandardDeviationInfoDto.SpecificationStatisticInfo
            {
                SpecificationName = specification.Name,
                data = circleList
            });
            oval.Add(new FilterStandardDeviationInfoDto.SpecificationStatisticInfo
            {
                SpecificationName = specification.Name,
                data = ovalList
            });
            length.Add(new FilterStandardDeviationInfoDto.SpecificationStatisticInfo
            {
                SpecificationName = specification.Name,
                data = lengthList
            });
            resistance.Add(new FilterStandardDeviationInfoDto.SpecificationStatisticInfo
            {
                SpecificationName = specification.Name,
                data = resistanceList
            });
            hardness.Add(new FilterStandardDeviationInfoDto.SpecificationStatisticInfo
            {
                SpecificationName = specification.Name,
                data = hardnessList
            });
        }


        result.Ids = groupBeginDate;
        result.Weight = weight;
        result.Circle = circle;
        result.Oval = oval;
        result.Length = length;
        result.Resistance = resistance;
        result.Hardness = hardness;
        return result;
    }

    private FilterMeasureDataDto getFilterMeasureDataList(IEnumerable<Data> dataList)
    {
        var weightList = new List<double>();
        var circleList = new List<double>();
        var ovalList = new List<double>();
        var lengthList = new List<double>();
        var resistanceList = new List<double>();
        var hardnessList = new List<double>();
        foreach (var item in dataList)
        {
            var data = JsonConvert.DeserializeObject<JObject>(item.Data);
            if (data[$"{_settings.Weight}"] != null)
            {
                var value = data[$"{_settings.Weight}"].ToString();
                if (!string.IsNullOrEmpty(value)) weightList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Circle}"] != null)
            {
                var value = data[$"{_settings.Circle}"].ToString();
                if (!string.IsNullOrEmpty(value)) circleList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Oval}"] != null)
            {
                var value = data[$"{_settings.Oval}"].ToString();
                if (!string.IsNullOrEmpty(value)) ovalList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Length}"] != null)
            {
                var value = data[$"{_settings.Length}"].ToString();
                if (!string.IsNullOrEmpty(value)) lengthList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Resistance}"] != null)
            {
                var value = data[$"{_settings.Resistance}"].ToString();
                if (!string.IsNullOrEmpty(value)) resistanceList.Add(double.Parse(value));
            }

            if (data[$"{_settings.Hardness}"] != null)
            {
                var value = data[$"{_settings.Hardness}"].ToString();
                if (!string.IsNullOrEmpty(value)) hardnessList.Add(double.Parse(value));
            }
        }

        var tempData = new FilterMeasureDataDto
        {
            WeightList = weightList,
            CircleList = circleList,
            OvalList = ovalList,
            LengthList = lengthList,
            ResistanceList = resistanceList,
            HardnessList = hardnessList
        };
        return tempData;
    }
}