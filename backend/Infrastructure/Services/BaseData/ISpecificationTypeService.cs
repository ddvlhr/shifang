using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Specification;

namespace Infrastructure.Services.BaseData;

public interface ISpecificationTypeService
{
    IEnumerable<SpecificationTypeTableDto> GetTable(BaseQueryInfoDto dto, out int total);
    bool Add(SpecificationTypeEditDto dto, out string failReason);
    bool Update(SpecificationTypeEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions();
    SpecificationTypeInfoDto GetSpecificationTypeInfo(int id);
}