using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Specification;

namespace Infrastructure.Services.Specification;

public interface IMeasureTypeIndicatorsService
{
    IEnumerable<MeasureTypeIndicatorTableDto> GetTable(BaseQueryInfoDto dto,
        out int total);

    bool Update(MeasureTypeIndicatorsEditDto dto, out string failReason);
    MeasureTypeIndicatorsEditDto GetInfo(int id);
}