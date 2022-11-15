using System.Collections.Generic;
using Core.Dtos;

namespace Infrastructure.Services.Defect;

public interface IDefectTypeService
{
    IEnumerable<BaseTableDto> GetDefectTypes(BaseQueryInfoDto dto, out int total);
    bool Edit(BaseEditDto dto, out string message);
    IEnumerable<BaseOptionDto> GetOptions();
}