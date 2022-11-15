using System.Collections.Generic;
using Core.Dtos;

namespace Infrastructure.Services.Indicator;

public interface IIndicatorParentService
{
    IEnumerable<BaseTableDto> Get(BaseQueryInfoDto dto, out int total);
    bool Add(BaseEditDto dto, out string failReason);
    bool Update(BaseEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetIndicatorParentOptions();
}