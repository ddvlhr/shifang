using System.Collections.Generic;
using Core.Dtos.GroupRecord;

namespace Core.Dtos.MetricalData;

public class FilterDataAddDto
{
    public int GroupId { get; set; }
    public List<int> GroupRecordIds { get; set; }
    public List<GroupRecordInfoDto> SelectedGroupRecords { get; set; }
}