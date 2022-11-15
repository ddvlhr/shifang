using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dtos.Specification;
using Core.Dtos.ProductWaterStatistic;
using Core.Entities;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Services.Statistics.Impl;

public class ProductWaterStatisticService : IProductWaterStatisticService
{
    private readonly IRepository<Data> _dRepo;
    private readonly IRepository<Group> _gRepo;
    private readonly IRepository<ProductReport> _prRepo;
    private readonly Settings _settings;
    private readonly IRepository<Core.Entities.Specification> _sRepo;

    public ProductWaterStatisticService(IRepository<ProductReport> prRepo, IRepository<Group> gRepo,
        IRepository<Data> dRepo, IRepository<Core.Entities.Specification> sRepo,
        IOptionsSnapshot<Settings> settings)
    {
        _prRepo = prRepo;
        _gRepo = gRepo;
        _dRepo = dRepo;
        _sRepo = sRepo;
        _settings = settings.Value;
    }

    public ProductWaterStatisticInfoDto GetInfo(ProductWaterStatisticQueryInfoDto dto)
    {
        var data = getInfo(dto);
        return data;
    }

    private ProductWaterStatisticInfoDto getInfo(ProductWaterStatisticQueryInfoDto dto)
    {
        var result = new ProductWaterStatisticInfoDto();

        var data = _gRepo.All().AsNoTracking().Where(c => c.MeasureTypeId == _settings.ProductionTypeId);

        if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
        {
            var typeId = int.Parse(dto.SpecificationTypeId);
            data = data.Where(c => c.Specification.SpecificationTypeId == typeId);
        }

        if (!string.IsNullOrEmpty(dto.BeginDate) && !string.IsNullOrEmpty(dto.EndDate))
        {
            var begin = Convert.ToDateTime(dto.BeginDate);
            var end = Convert.ToDateTime(dto.EndDate);
            data = data.Where(c => c.BeginTime.Date >= begin && c.EndTime.Date <= end);
        }

        var tempData = data.Include(c => c.Specification).ToList();
        var rules = _sRepo.All().AsNoTracking().Select(c => new { c.Id, c.SingleRules }).ToList();
        var groups = tempData.GroupBy(c => c.SpecificationId).ToList();
        var timeX = new List<string>();
        var list = new List<ProductWaterStatisticInfoDto.ProductWaterInfo>();
        var rates = new List<string>();
        foreach (var g in groups)
        {
            var first = g.FirstOrDefault();
            if (first == null)
                continue;
            var rule = rules.FirstOrDefault(c => c.Id == g.Key);
            var singleRules = JsonConvert.DeserializeObject<List<Rule>>(rule.SingleRules);
            var waterRule = singleRules.FirstOrDefault(c => c.Id == _settings.WaterIndicatorId);
            if (waterRule == null)
                continue;

            var waterList = new List<double>();
            var timeWaterInfos = new List<string[]>();
            var groupIds = g.Select(c => c.Id).ToList();
            var dataList = _dRepo.All().Where(c => groupIds.Contains(c.GroupId))
                .Select(c => new { c.GroupId, c.Data }).ToList();
            foreach (var group in g.OrderBy(c => c.BeginTime))
            {
                var tempDataList = dataList.Where(c => c.GroupId == group.Id);
                foreach (var temp in tempDataList)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(temp.Data);
                    var water = "";
                    if (obj[_settings.WaterIndicatorId.ToString()] != null &&
                        !string.IsNullOrEmpty(obj[_settings.WaterIndicatorId.ToString()].ToString()))
                        water = obj[_settings.WaterIndicatorId.ToString()].ToString();

                    if (!string.IsNullOrEmpty(water))
                    {
                        var time = group.BeginTime.ToString("yyyy-MM-dd HH:mm:ss");
                        timeWaterInfos.Add(new[] { time, water });
                        timeX.Add(time);
                        waterList.Add(double.Parse(water));
                    }
                }
            }

            var statistic = waterList.toStatistic(double.Parse(waterRule.Standard), double.Parse(waterRule.Upper),
                double.Parse(waterRule.Lower), waterRule.Equal);
            var item = new ProductWaterStatisticInfoDto.ProductWaterInfo
            {
                SpecificationName = $"{first.Specification.Name}: {statistic.QualityRate}"
            };
            item.WaterInfos = timeWaterInfos;
            list.Add(item);
        }

        result.Rates = rates;
        result.TimeX = timeX.Distinct().OrderBy(c => c).ToList();
        result.ProductWaterInfos = list;
        return result;
    }
}