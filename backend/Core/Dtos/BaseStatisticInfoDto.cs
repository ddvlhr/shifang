using System.Collections.Generic;

namespace Core.Dtos;

public class BaseStatisticInfoDto
{
    public Dictionary<string, object> Columns { get; set; }
    public Dictionary<string, object> StatisticColumns { get; set; }
    public List<Dictionary<string, object>> DataInfo { get; set; }
    public List<Dictionary<string, object>> StatisticDataInfo { get; set; }
}