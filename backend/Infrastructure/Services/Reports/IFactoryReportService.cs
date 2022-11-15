using System.Collections.Generic;
using System.IO;
using Core.Dtos.MetricalData;
using Core.Dtos.Report;

namespace Infrastructure.Services.Reports;

public interface IFactoryReportService
{
    IEnumerable<ReportTableDto> GetTable(QueryInfoDto dto, out int total);
    bool Add(MetricalDataEditDataDto dto);
    FactoryReportEditDto GetInfo(int id);
    bool Edit(FactoryReportEditDto dto);
    MemoryStream Download(int id);
    FactoryReportDownloadDto GetReportInfo(int id);
}