namespace Core.Dtos.Report;

public class ProcessDisciplineReportQueryInfoDto : BaseQueryInfoDto
{
    public string ClassId { get; set; }
    public string ClauseId { get; set; }
    public string Department { get; set; }
}