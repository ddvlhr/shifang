namespace Core.Dtos.WorkShopQuality;

public class WorkShopQualityQueryInfoDto
{
    public string WorkShopId { get; set; }
    public string SpecificationTypeId { get; set; }
    public string MachineModelId { get; set; }
    public string BeginDate { get; set; }
    public string EndDate { get; set; }
}