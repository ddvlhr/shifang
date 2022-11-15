namespace Core.Dtos.WorkShopQuality;

public class WorkShopQualityPointRuleEditDto
{
    public int Id { get; set; }
    public int WorkShopId { get; set; }
    public int SpecificationTypeId { get; set; }
    public string PhysicalPointPercent { get; set; }
    public string PhysicalAllPercent { get; set; }
    public string InspectionPointPercent { get; set; }
    public string InspectionAllPercent { get; set; }
    public string ProductPointPercent { get; set; }
    public string ProductAllPercent { get; set; }
    public bool State { get; set; }
}