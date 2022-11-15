using Core.Enums;

namespace Core.Dtos.Statistics;

public class StatisticQueryInfoDto
{
    public string SpecificationId { get; set; }
    public string TurnId { get; set; }
    public string MachineId { get; set; }
    public string BeginDate { get; set; }
    public string EndDate { get; set; }
    public string ExcludeDate { get; set; }
    public CustomerEnum.FilterStatisticPlotType StatisticType { get; set; }
}