namespace Core.Dtos.Specification;

public class SpecificationTableDto : BaseTableDto
{
    public string TypeName { get; set; }
    public int TypeId { get; set; }
    public string OrderNo { get; set; }
    public string ModifiedTime { get; set; }
}