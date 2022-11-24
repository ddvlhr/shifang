namespace Core.Dtos.WrapProcessInspectionReport;

public class WrapProcessInspectionReportQueryInfoDto: BaseQueryInfoDto
{
    public string SpecificationId { get; set; }
    public string MachineId { get; set; }
    public string TurnId { get; set; }
}