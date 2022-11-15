namespace Core.Dtos.GroupRecord;

public class GroupRecordQueryInfoDto : BaseQueryInfoDto
{
    public string BeginTime { get; set; }
    public string EndTime { get; set; }
    public string SpecificationId { get; set; }
    public string MeasureTypeId { get; set; }
    public string TurnId { get; set; }
    public string MachineId { get; set; }
    public string GroupId { get; set; }
}