using System.Collections.Generic;
using Core.Dtos.MetricalData;

namespace Core.Dtos.Report;

public class ReportStatisticDto
{
    public string SpecificationName { get; set; }
    public string TurnName { get; set; }
    public string MachineModelName { get; set; }
    public string MachineName { get; set; }
    public string StatisticColumns { get; set; }
    public string StatisticData { get; set; }
    public string OriginColumns { get; set; }
    public string OriginData { get; set; }
    public int Points { get; set; }
    public List<string> PhyRetDes { get; set; }
    public List<string> Appearances { get; set; }
    public bool PhyRetState { get; set; }
    public string PhyRet { get; set; }
    public bool FinalRetState { get; set; }
    public string FinalRet { get; set; }
    public string Total { get; set; }
    public List<WaterRecordTableDto> WaterInfos { get; set; }
}