namespace Core.Dtos.MetricalData;

public class MetricalDataGroupEditDto
{
    public int Id { get; set; }
    public string TestTime { get; set; }
    public string ProductionTime { get; set; }
    public string DeliverTime { get; set; }
    public int SpecificationId { get; set; }
    public int TurnId { get; set; }
    public int MachineModelId { get; set; }
    public int MeasureTypeId { get; set; }
    public string OrderNo { get; set; }
    public string Instance { get; set; }
    public int CopyId { get; set; }
}