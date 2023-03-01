namespace Core.Dtos.MetricalData;

public class MetricalDataQueryDto : BaseQueryInfoDto
{
    public string SpecificationId { get; set; }
    public string SpecificationTypeId { get; set; }
    public string TurnId { get; set; }
    public string MachineModelId { get; set; }
    public string MeasureTypeId { get; set; }
    public string BeginTime { get; set; }
    public string EndTime { get; set; }
    public string EquipmentTypeId { get; set; }
}