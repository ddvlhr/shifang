using System.Collections.Generic;

namespace Core.Dtos.GroupRecord;

public class GroupInfoDto
{
    public IEnumerable<GroupRecordInfoDto> FromRecords { get; set; }
    public IEnumerable<GroupRecordInfoDto> GroupRecords { get; set; }
}

public class GroupRecordInfoDto
{
    public int GroupId { get; set; }
    public string TestTime { get; set; }
    public string SpecificationName { get; set; }
    public string TurnName { get; set; }
    public string MachineName { get; set; }
    public string MeasureTypeName { get; set; }
    public string IndicatorName { get; set; }
    public int Count { get; set; }
}