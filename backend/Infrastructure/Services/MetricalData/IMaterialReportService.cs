using System.Collections.Generic;
using Core.Dtos.Material;
using Core.Dtos.MetricalData;

namespace Infrastructure.Services.MetricalData;

public interface IMaterialReportService
{
    IEnumerable<MaterialReportTableDto> GetTable(MaterialReportQueryInfoDto dto, out int total);
    bool Edit(MaterialReportEditDto dto, out string failReason);
    string GetTemplate(int id);
    MaterialReportEditDto GetInfo(int id);
    bool Add(MetricalDataEditDataDto dto);
}