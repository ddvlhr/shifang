namespace Core.Dtos.MetricalData;

public class MetricalDataTableDto
{
    public int Id { get; set; }
    public int SpecificationId { get; set; }
    public string SpecificationName { get; set; }
    public int SpecificationTypeId { get; set; }
    public int? TurnId { get; set; }
    public string TurnName { get; set; }
    public int? MachineModelId { get; set; }
    public string MachineModelName { get; set; }
    public int? MachineId { get; set; }
    public string MachineName { get; set; }
    public int MeasureTypeId { get; set; }
    public string MeasureTypeName { get; set; }
    public string BeginTime { get; set; }
    public string EndTime { get; set; }
    public string ProductionTime { get; set; }
    public string DeliverTime { get; set; }
    public string OrderNo { get; set; }
    public string Instance { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
}