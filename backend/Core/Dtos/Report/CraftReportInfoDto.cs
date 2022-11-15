using System.Collections.Generic;

namespace Core.Dtos.Report;

public class CraftReportInfoDto
{
    public string SpecificationName { get; set; }
    public string WorkShopName { get; set; }
    public string TestDate { get; set; }
    public string OrderNo { get; set; }
    public string Score { get; set; }
    public string ModelName { get; set; }
    public string TurnName { get; set; }
    public string TemperatureInfo { get; set; }
    public string ControlInfo { get; set; }
    public string UserName { get; set; }
    public string LogOrderNo { get; set; }
    public IEnumerable<CraftTestItem> CraftTestItems { get; set; }

    public class CraftTestItem
    {
        public string MachineModelName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}