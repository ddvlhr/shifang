namespace Core.Dtos.Report;

public class QueryInfoDto : BaseQueryInfoDto
{
    public string SpecificationId { get; set; }
    public string SpecificationTypeId { get; set; }
    public string TurnId { get; set; }
    public string MachineModelId { get; set; }
    public string TestDate { get; set; }
    public string BeginDate { get; set; }
    public string EndDate { get; set; }
}