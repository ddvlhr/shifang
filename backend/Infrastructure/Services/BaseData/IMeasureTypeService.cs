using System.Collections.Generic;
using Core.Dtos;

namespace Infrastructure.Services.BaseData;

public interface IMeasureTypeService
{
    IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total);
    bool Add(BaseEditDto dto, out string failReason);
    bool Update(BaseEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions();
    int GetTypeIdByTypeName(string typeName);
}