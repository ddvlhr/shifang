using Core.Dtos.Report;

namespace Infrastructure.Services.Reports;

public interface IProductReportAppearanceService
{
    bool Update(EditReportAppearanceDto dto, out string failReason);
    EditReportAppearanceDto GetReportAppearances(int id);
    int GetScore(int id);
}