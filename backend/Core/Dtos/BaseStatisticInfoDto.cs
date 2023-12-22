using System.Collections.Generic;
using Core.Dtos.Specification;

namespace Core.Dtos;

public class BaseStatisticInfoDto
{
    public string SpecificationName { get; set; }
    public int MachineId { get; set; }
    public string BeginTime { get; set; }
    public string WorkShopName { get; set; }
    public string Instance { get; set; }
    public Dictionary<string, DoubleRule> Standard { get; set; }
    public Dictionary<string, object> Columns { get; set; }
    public Dictionary<string, object> StatisticColumns { get; set; }
    public List<Dictionary<string, object>> DataInfo { get; set; }
    public List<Dictionary<string, Dictionary<string, object>>> StatisticDataInfo { get; set; }
    public Dictionary<string, List<double>> ChartDataInfo { get; set; }
    public List<Dictionary<string, Dictionary<string, object>>> DayStatisticDataInfo { get; set; }
    public Dictionary<string, Dictionary<string, int>> ChartMarkLineInfo { get; set; }
}