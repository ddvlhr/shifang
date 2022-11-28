using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Core.Dtos.MaterialCheckReport;

public class MaterialCheckReportInfoDto
{
    public int Id { get; set; }
    public string TestDate { get; set; }
    public int SpecificationId { get; set; }
    public string SpecificationName { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public int TurnId { get; set; }
    public string TurnName { get; set; }
    public int MachineId { get; set; }
    public string MachineName { get; set; }
    public int MeasureTypeId { get; set; }
    public string MeasureTypeName { get; set; }
    public int GroupId { get; set; }
    public string Originator { get; set; }
    public string Operator { get; set; }
    public int Qualified { get; set; }
    public int MaterialCheckStatus { get; set; }
    public string TempData { get; set; }
    public List<Dictionary<string, object>> DataInfo { get; set; } = new List<Dictionary<string, object>>();
    public string Data { get; set; }
}