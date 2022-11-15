namespace Core.Dtos.Specification;

public class SpecificationTypeTableDto : BaseTableDto
{
    public string ProductOrderNo { get; set; }
    public string InspectionOrderNo { get; set; }
    public string PhysicalOrderNo { get; set; }
    public string MaterialOrderNo { get; set; }
    public string FactoryOrderNo { get; set; }
    public string CraftOrderNo { get; set; }
}