using System.Collections.Generic;
using System.IO;
using Core.Dtos.Statistics;

namespace Infrastructure.Services.Statistics;

public interface IFilterMeasureQualityService
{
    IEnumerable<FilterMeasureQualityInfoDto> Search(StatisticQueryInfoDto dto);
    MemoryStream Download(StatisticQueryInfoDto dto);
}