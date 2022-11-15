using System.Collections.Generic;
using Core.Dtos;

namespace Infrastructure.Services.BaseData;

public interface IMachineService
{
    IEnumerable<BaseTableDto> GetTable(BaseQueryInfoDto dto, out int total);
    bool Add(BaseEditDto dto, out string failReason);
    bool Update(BaseEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions();
}