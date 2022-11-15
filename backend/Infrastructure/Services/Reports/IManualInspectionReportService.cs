using System.Collections.Generic;
using Core.Dtos.ManualInspectionReport;

namespace Infrastructure.Services.Reports;

public interface IManualInspectionReportService
{
    IEnumerable<ManualInspectionReportInfoDto> GetReports(ManualInspectionReportQueryInfoDto dto, out int total);
    bool Edit(ManualInspectionReportInfoDto dto, out string message);
    bool Remove(List<int> ids, out string message);
}