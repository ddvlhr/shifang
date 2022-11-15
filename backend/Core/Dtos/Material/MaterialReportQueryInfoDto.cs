namespace Core.Dtos.Material;

public class MaterialReportQueryInfoDto : BaseQueryInfoDto
{
    public string TypeId { get; set; }
    public string ManufacturerId { get; set; }
}