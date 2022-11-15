namespace Core.Dtos.Defect;

public class DefectInfoDto
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; }
    public int EventId { get; set; }
    public string EventName { get; set; }
    public string ShortName { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public double Score { get; set; }
    public bool State { get; set; }
}