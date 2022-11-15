using System.Collections.Generic;
using Core.Dtos;

namespace Infrastructure.Services.BaseData;

public interface IManufacturerService
{
    IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total);
    bool Add(BaseEditDto dto, out string failReason);
    bool Update(BaseEditDto dto, out string failReason);
    bool Delete(IEnumerable<int> ids);
    IEnumerable<BaseOptionDto> GetOptions();
}