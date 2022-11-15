using System.Collections.Generic;
using System.IO;
using Core.Dtos.WorkShopQuality;

namespace Infrastructure.Services.Statistics;

public interface IWorkShopQualityService
{
    IEnumerable<WorkShopQualityInfoDto> GetInfo(WorkShopQualityQueryInfoDto dto);
    MemoryStream Download(WorkShopQualityQueryInfoDto dto);
}