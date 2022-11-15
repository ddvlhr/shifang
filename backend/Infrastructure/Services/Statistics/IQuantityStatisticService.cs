using System.Collections.Generic;
using System.IO;
using Core.Dtos.QuantityStatistic;

namespace Infrastructure.Services.Statistics;

public interface IQuantityStatisticService
{
    IEnumerable<QuantityStatisticInfoDto> GetInfo(QuantityStatisticQueryInfoDto dto);
    MemoryStream Download(QuantityStatisticQueryInfoDto dto);
}