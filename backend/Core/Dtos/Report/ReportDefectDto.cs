namespace Core.Dtos.Report;

public class ReportDefectDto
{
    public int Id { get; set; }
    public int DefectId { get; set; }
    public int DefectCategory { get; set; }
    public string DefectShortName { get; set; }
    public int Count { get; set; }
}