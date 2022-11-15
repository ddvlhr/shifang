using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Core.Dtos.Report;

public class ReportDownloadDto
{
    public string SpecificationName { get; set; }
    public int SpecificationRowCount { get; set; }
    public string MachineModelName { get; set; }
    public int MachineModelRowCount { get; set; }
    public string MeasureTime { get; set; }
    public JArray MeasureInfo { get; set; }
    public List<Dictionary<string, object>> MeaInfos { get; set; }
    public double MeanTotal { get; set; }
    public JArray AppearanceInfo { get; set; }
    public int Total { get; set; }
    public string Ret { get; set; }
    public List<string> MeaColumns { get; set; }
    public List<string> AppColumns { get; set; }
}