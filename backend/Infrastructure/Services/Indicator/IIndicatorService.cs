using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Indicator;
using Core.Entities;
using Core.Dtos.Specification;

namespace Infrastructure.Services.Indicator;

public interface IIndicatorService
{
    IEnumerable<IndicatorTableDto> Get(IndicatorQueryInfoDto dto,
        out int total);

    bool Add(IndicatorEditDto dto, out string failReason);
    bool Update(IndicatorEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetIndicatorOptions(IndicatorProject project);
    IEnumerable<Rule> GetRuleList(List<int> ids);
}