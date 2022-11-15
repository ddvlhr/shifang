namespace Core.Dtos.Material;

public class MaterialReportTableDto
{
    public int Id { get; set; }
    public string TestDate { get; set; }
    public string TypeName { get; set; }
    public string SpecificationName { get; set; }
    public string Manufacturer { get; set; }
    public string SamplePlace { get; set; }
    public int State { get; set; }
    public string UserName { get; set; }
}