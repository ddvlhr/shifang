namespace Core.Dtos.Report;

public class ProcessDisciplineReportInfoDto
{
    public int Id { get; set; }
    public string Time { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string Description { get; set; }
    public string Reward { get; set; }
    public string Punishment { get; set; }
}