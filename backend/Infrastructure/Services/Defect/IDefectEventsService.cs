using System.Collections.Generic;
using Core.Dtos;

namespace Infrastructure.Services.Defect;

public interface IDefectEventsService
{
    IEnumerable<BaseTableDto> GetDefectEvents(BaseQueryInfoDto dto, out int total);
    bool Edit(BaseEditDto dto, out string message);
    IEnumerable<BaseOptionDto> GetOptions();
}