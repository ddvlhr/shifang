using System.Collections.Generic;
using Core.Dtos.MaterialCheckReport;

namespace Infrastructure.Services.Reports;

public interface IMaterialCheckReportService
{
    IEnumerable<MaterialCheckReportInfoDto> GetReports(MaterialCheckReportQueryInfoDto dto, out int total);
    bool EditReport(MaterialCheckReportInfoDto dto, out string message);
    bool RemoveReports(List<int> ids, out string message);
}