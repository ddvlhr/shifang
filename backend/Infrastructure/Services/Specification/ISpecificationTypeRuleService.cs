using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Specification;

namespace Infrastructure.Services.Specification;

public interface ISpecificationTypeRuleService
{
    IEnumerable<SpecificationTypeRuleTableDto> GetTable(BaseQueryInfoDto dto, out int total);
    bool Edit(SpecificationTypeRuleEditDto dto, out string failReason);
    SpecificationTypeRuleEditDto Get(int id);
}