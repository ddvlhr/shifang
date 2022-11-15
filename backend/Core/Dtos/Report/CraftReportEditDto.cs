namespace Core.Dtos.Report;

public class CraftReportEditDto
{
    public int Id { get; set; }
    public string SpecificationName { get; set; }
    public string MachineModelName { get; set; }
    public string ModelName { get; set; }
    public double Score { get; set; }
    public string Remark { get; set; }
    public string OrderNo { get; set; }
    public bool State { get; set; }
    public string Temperature { get; set; }
    public string ControlSituation { get; set; }
    public string LogOrderNo { get; set; }
}