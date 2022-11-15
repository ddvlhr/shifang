using System.Collections.Generic;
using System.IO;
using Core.Dtos.MetricalData;
using Core.Dtos.Report;

namespace Infrastructure.Services.Reports
{
    public interface ITestReportService
    {
        bool Add(AddReportDto dto);
        bool Add(MetricalDataEditDataDto dto);
        IEnumerable<ReportTableDto> GetTable(QueryInfoDto dto, out int total);
        ReportStatisticDto GetStatisticInfo(int id, out string failReason);
        MemoryStream Download(List<int> ids);
    }
}