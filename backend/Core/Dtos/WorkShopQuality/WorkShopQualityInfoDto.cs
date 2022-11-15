namespace Core.Dtos.WorkShopQuality;

public class WorkShopQualityInfoDto
{
    public int WorkShopId { get; set; }
    public string WorkShopName { get; set; }
    public int WorkShopNameRowCount { get; set; } = 0;
    public string MachineModelName { get; set; }
    public int MachineModelNameRowCount { get; set; } = 0;
    public int SpecificationTypeId { get; set; }
    public string SpecificationTypeName { get; set; }
    public int SpecificationTypeNameRowCount { get; set; } = 0;
    public int TurnId { get; set; }
    public string TurnName { get; set; }
    public int TurnNameRowCount { get; set; }
    public int InspectionQualityValue { get; set; }
    public string InspectionQualityValueStr { get; set; }
    public int PhysicalQualityValue { get; set; }
    public string PhysicalQualityValueStr { get; set; }
    public int ProductQualityValue { get; set; }
    public string ProductQualityValueStr { get; set; }
    public string MachineQualityValueAverage { get; set; }
    public string TurnQualityValueAverage { get; set; }

    public string WorkShopQualityValueAverage { get; set; }
}