namespace Core.Dtos.ManualInspectionReport;

public class ManualInspectionReportQueryInfoDto: BaseQueryInfoDto
{
    public string SpecificationId { get; set; }
    public string Result { get; set; }
}