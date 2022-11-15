using System.Collections.Generic;
using Core.Dtos.WrapQualityInspectionReport;

namespace Infrastructure.Services.Reports;

public interface IWrapQualityInspectionReportService
{
    IEnumerable<WrapQualityInspectionReportInfoDto> GetReport(WrapQualityInspectionReportQueryInfoDto dto, out int total);
    bool EditReport(WrapQualityInspectionReportInfoDto dto, out string message);
    bool RemoveReport(List<int> ids, out string message);
}