using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.WorkShop;

namespace Infrastructure.Services.BaseData;

public interface IWorkShopService
{
    IEnumerable<WorkShopTableDto> GetTable(BaseQueryInfoDto dto, out int total);
    bool Add(BaseEditDto dto, out string failReason);
    bool Update(BaseEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions(bool stringValue);
}