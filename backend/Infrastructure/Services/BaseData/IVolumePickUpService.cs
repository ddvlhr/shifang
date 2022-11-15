using System.Collections.Generic;
using Core.Dtos;

namespace Infrastructure.Services.BaseData;

public interface IVolumePickUpService
{
    IEnumerable<BaseTableDto> GetVolumePickUps(BaseQueryInfoDto dto, out int total);
    bool Edit(BaseEditDto dto, out string message);
    IEnumerable<BaseOptionDto> GetOptions();
}