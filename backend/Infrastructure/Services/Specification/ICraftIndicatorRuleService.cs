using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Specification;

namespace Infrastructure.Services.Specification;

public interface ICraftIndicatorRuleService
{
    IEnumerable<CraftIndicatorTableDto> GetTable(BaseQueryInfoDto dto, out int total);
    bool Edit(CraftIndicatorEditDto dto, out string failReason);
    CraftIndicatorEditDto Get(int id);
}