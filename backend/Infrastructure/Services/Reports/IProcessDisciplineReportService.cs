using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Dtos.Report;
using Core.Models;
using Core.SugarEntities;

namespace Infrastructure.Services.Reports;

public interface IProcessDisciplineReportService
{
    Task<PageViewModel<ProcessDisciplineReportInfoDto>> GetTableAsync(ProcessDisciplineReportQueryInfoDto dto);
    Task<ResultViewModel<ProcessDisciplineReportInfoDto>> GetByIdAsync(int id);
    Task<ResultViewModel<ProcessDisciplineReportInfoDto>> EditAsync(ProcessDisciplineReportInfoDto dto);
    Task<ResultViewModel<ProcessDisciplineReport>> RemoveAsync(List<int> ids);
    Task<MemoryStream> DownloadAsync(ProcessDisciplineReportQueryInfoDto dto);
}