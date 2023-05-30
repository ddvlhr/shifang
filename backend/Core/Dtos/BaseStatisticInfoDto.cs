using System.Collections.Generic;
using Core.Dtos.Specification;
using Ubiety.Dns.Core.Records;

namespace Core.Dtos;

public class BaseStatisticInfoDto
{
    public Dictionary<string, DoubleRule> Standard { get; set; }
    public Dictionary<string, object> Columns { get; set; }
    public Dictionary<string, object> StatisticColumns { get; set; }
    public List<Dictionary<string, object>> DataInfo { get; set; }
    public List<Dictionary<string, Dictionary<string, object>>> StatisticDataInfo { get; set; }
    public Dictionary<string, List<double>> ChartDataInfo { get; set; }
}