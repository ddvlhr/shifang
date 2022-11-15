using Core.Dtos.Report;

namespace Infrastructure.Services.Reports;

public interface IPhysicalReportAppearanceService
{
    bool Update(EditReportAppearanceDto dto, out string failReason);
    EditReportAppearanceDto GetReportAppearances(int id);
}