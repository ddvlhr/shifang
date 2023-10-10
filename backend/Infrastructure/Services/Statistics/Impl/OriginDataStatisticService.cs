using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos.Specification;
using Core.Dtos.Statistics;
using Core.Entities;
using Core.Enums;
using Core.Models;
using Core.SugarEntities;
using Infrastructure.Attributes;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Infrastructure.Services.MetricalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace Infrastructure.Services.Statistics.Impl
{
    [AutoInject(typeof(IOriginDataStatisticService), InjectType.Scope)]
    public class OriginDataStatisticService: SugarRepository<MetricalGroup>, IOriginDataStatisticService
    {
        private readonly ISqlSugarClient _db;
        private readonly IRepository<Core.Entities.Indicator> _iRepo;
        private readonly IRepository<Core.Entities.Specification> _spRepo;
        private readonly IRepository<Data> _dRepo;
        private readonly Core.Models.ShiFangSettings _settings;

        public OriginDataStatisticService(ISqlSugarClient db, 
            IRepository<Core.Entities.Indicator> iRepo,
            IOptionsSnapshot<Core.Models.ShiFangSettings> settings,
            IRepository<Core.Entities.Specification> spRepo,
            IRepository<Data> dRepo)
        {
            _db = db;
            _iRepo = iRepo;
            _spRepo = spRepo;
            _dRepo = dRepo;
            _settings = settings.Value;
        }
        public IEnumerable<OriginDataStatisticDto.OriginDataStatisticDataDto> Search(OriginDataStatisticDto.OriginDataStatisticQueryDto dto)
        {
            // var exp = Expressionable
            // .Create<Core.SugarEntities.MetricalData, MetricalGroup, Core.SugarEntities.Specification>()
            // .AndIF(dto.SpecificationId != null,
            //     (c, g, s) => g.SpecificationId == int.Parse(dto.SpecificationId))
            // .AndIF(dto.Begin != null && dto.End != null,
            //     (c, g, s) => g.BeginTime.Date >= Convert.ToDateTime(dto.Begin).Date &&
            //                                 g.BeginTime.Date <= Convert.ToDateTime(dto.End).Date)
            // .AndIF(dto.SpecificationTypeId != null,
            //     (c, g, s) => s.SpecificationTypeId == int.Parse(dto.SpecificationTypeId))
            // .ToExpression();
            // var list = _db
            //     .Queryable<Core.SugarEntities.MetricalData, MetricalGroup, Core.SugarEntities.Specification>((c, g, s) => new JoinQueryInfos(
            //         JoinType.Left, c.GroupId == g.Id,
            //         JoinType.Left, g.SpecificationId == s.Id
            //     )).Where(exp).Select((c, g, s) => new
            //     {
            //         c.Id,
            //         c.GroupId,
            //         specificationId = g.SpecificationId,
            //         specificationName = s.Name,
            //         testTime = c.TestTime,
            //         data = c.Data,
            //         userData = g.UserData,
            //         instance = g.Instance,
            //         equipmentType = g.EquipmentType
            //     }).ToList();

            var temp = _dRepo.All().Include(c=>c.Group).ThenInclude(c=>c.Specification).AsNoTracking();
            if (!string.IsNullOrEmpty(dto.SpecificationId))
            {
                var specificationId = int.Parse(dto.SpecificationId);
                temp = temp.Where(c => c.Group.SpecificationId == specificationId);
            }

            if (!string.IsNullOrEmpty(dto.SpecificationTypeId))
            {
                var specificationTypeId = int.Parse(dto.SpecificationTypeId);
                temp = temp.Where(c => c.Group.Specification.SpecificationTypeId == specificationTypeId);
            }

            if (!string.IsNullOrEmpty(dto.Begin) && !string.IsNullOrEmpty(dto.End))
            {
                var begin = Convert.ToDateTime(dto.Begin);
                var end = Convert.ToDateTime(dto.End);
                temp = temp.Where(c => c.Group.BeginTime.Date >= begin.Date && c.Group.BeginTime.Date <= end.Date);
            }

            var list = temp.Where(c=>c.Group.EquipmentType == EquipmentType.SingleResistance).Select(c=> new
            {
                c.Id,
                c.GroupId,
                specificationId = c.Group.SpecificationId,
                specificationName = c.Group.Specification.Name,
                testTime = c.TestTime,
                data = c.Data,
                userData = c.Group.UserData,
                instance = c.Group.Instance,
                equipmentType = c.Group.EquipmentType
            }).ToList();

            var groups = list.GroupBy(c => c.specificationId).ToList();
            var indicators = _iRepo.All().ToList();
            var specifications = _spRepo.All().Where(c=>c.EquipmentType == EquipmentType.SingleResistance).ToList();
            var result = new List<OriginDataStatisticDto.OriginDataStatisticDataDto>();
            foreach (var group in groups)
            {
                var item = new OriginDataStatisticDto.OriginDataStatisticDataDto();
                var specification = specifications.FirstOrDefault(c => c.Id == group.Key);
                var rules = JsonConvert.DeserializeObject<List<Rule>>(specification?.SingleRules);
                var resistanceRule = rules.FirstOrDefault(c => c.Id == _settings.Resistance);
                var tempResistanceList = new List<double>();
                var chartList = new List<double>();
                foreach (var data in group)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(data.data);
                    double resistance = 0;
                    if (obj[_settings.Resistance.ToString()] != null &&
                        !string.IsNullOrEmpty(obj[_settings.Resistance.ToString()].ToString()))
                    {
                        resistance = double.Parse(obj[_settings.Resistance.ToString()].ToString());
                    }

                    if (resistance > 0)
                    {
                        tempResistanceList.Add(resistance);
                        chartList.Add(resistance);
                    }
                }

                var statistic = tempResistanceList.toStatistic(resistanceRule.Standard, resistanceRule.Upper,
                    resistanceRule.Lower);
                item.SpecificationId = specification.Id;
                item.SpecificationName = specification.Name;
                item.Mean = statistic.Mean.toString(_settings.IndicatorDecimal.Mean);
                item.Total = statistic.TotalCount.ToString();
                item.Rate = statistic.QualityRate;
                item.Quality = statistic.Quality;
                item.QualityCount = statistic.QualityCount.ToString();
                item.List = chartList;
                item.Rule = resistanceRule;
                result.Add(item);
            }

            return result;
        }
    }
}
