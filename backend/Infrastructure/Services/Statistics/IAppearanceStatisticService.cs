using System.Collections.Generic;
using System.IO;
using Core.Dtos.AppearanceStatistic;

namespace Infrastructure.Services.Statistics;

public interface IAppearanceStatisticService
{
    IEnumerable<AppearanceStatisticInfoDto> GetInfo(QueryInfoDto dto);
    MemoryStream DownloadData(QueryInfoDto dto);
}