using System.Collections.Generic;
using System.IO;
using Core.Dtos.Report;

namespace Infrastructure.Services.Reports;

public interface IProcessDisciplineReportService
{
    IEnumerable<ProcessDisciplineReportInfoDto> GetTable(ProcessDisciplineReportQueryInfoDto dto, out int total);
    bool Edit(ProcessDisciplineReportInfoDto dto, out string message);
    bool Remove(List<int> ids, out string message);
    MemoryStream Download(ProcessDisciplineReportQueryInfoDto dto);
}