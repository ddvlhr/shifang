using System;
using System.Collections.Generic;
using System.IO;
using Core.Dtos.AppearanceDistribute;

namespace Infrastructure.Services.Statistics;

public interface IAppearanceDistributeService
{
    IEnumerable<AppearanceDataDto> Search(DateTime beginDate, DateTime endDate, int typeId);
    MemoryStream Download(DateTime beginDate, DateTime endDate, int typeId);
}