namespace Core.Dtos.MaterialCheckReport;

public class MaterialCheckReportQueryInfoDto: BaseQueryInfoDto
{
    public string SpecificationId { get; set; }
    public string TeamId { get; set; }
    public string TurnId { get; set; }
    public string MachineId { get; set; }
    public string MeasureTypeId { get; set; }
    public string Qualified { get; set; }
}