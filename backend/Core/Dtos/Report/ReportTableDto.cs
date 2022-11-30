using System.Collections.Generic;
using Core.Dtos.ProductReport;
using Core.Entities;

namespace Core.Dtos.Report;

public class ReportTableDto
{
    public int Id { get; set; }
    public string BeginTime { get; set; }
    public string SpecificationName { get; set; }
    public int SpecificationId { get; set; }
    public string ProductDate { get; set; }
    public string TurnName { get; set; }
    public string MachineModelName { get; set; }
    public string ModelName { get; set; }
    public int FinalRet { get; set; }
    public string UserName { get; set; }
    public string Remark { get; set; }
    public int PhyScore { get; set; }
    public int TypeId { get; set; }
    public List<ReportDefectDto> Defects { get; set; }
}