using System.Collections.Generic;
using Core.Dtos.Report;
using QueryInfoDto = Core.Dtos.MonthCraftReport.QueryInfoDto;

namespace Infrastructure.Services.Reports;

public interface IMonthCraftReportService
{
    IEnumerable<MonthCraftReportTableDto> GetTable(QueryInfoDto dto, out int total);
    bool Add(MonthCraftReportEditDto dto, out string failReason);
    bool Update(MonthCraftReportEditDto dto, out string failReason);
    MonthCraftReportEditDto GetInfo(int id, out string failReason);
    bool Remove(List<int> ids, out string failReason);
}