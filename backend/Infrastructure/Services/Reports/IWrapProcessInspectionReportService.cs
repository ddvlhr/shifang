using System.Collections.Generic;
using System.IO;
using Core.Dtos.WrapProcessInspectionReport;

namespace Infrastructure.Services.Reports;

public interface IWrapProcessInspectionReportService
{
    IEnumerable<WrapProcessInspectionReportInfoDto> GetReports(WrapProcessInspectionReportQueryInfoDto dto,
        out int total);

    bool EditReport(WrapProcessInspectionReportInfoDto dto, out string message);
    bool RemoveReports(List<int> ids, out string message);
    MemoryStream DownloadReports(WrapProcessInspectionReportQueryInfoDto dto);
}