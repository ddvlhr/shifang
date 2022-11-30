using System.Collections.Generic;
using System.IO;
using Core.Dtos.ManualInspectionReport;
using Core.Dtos.ManualTestStatistic;

namespace Infrastructure.Services.Statistics;

public interface IManualTestStatisticService
{
    ManualTestStatisticInfoDto Search(ManualInspectionReportQueryInfoDto dto);
    MemoryStream Download(ManualInspectionReportQueryInfoDto dto);
}