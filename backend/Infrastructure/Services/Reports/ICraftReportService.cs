using System.Collections.Generic;
using Core.Dtos.MetricalData;
using Core.Dtos.Report;

namespace Infrastructure.Services.Reports;

public interface ICraftReportService
{
    IEnumerable<ReportTableDto> GetTable(QueryInfoDto dto, out int total);
    bool Add(MetricalDataEditDataDto dto, out string failReason);
    bool Edit(CraftReportEditDto dto);
    CraftReportEditDto GetInfo(int id);
    CraftReportInfoDto GetReportInfo(int id);
}