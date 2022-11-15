using System.Collections.Generic;

namespace Core.Dtos.Report;

public class FactoryReportEditDto
{
    public int Id { get; set; }
    public List<int> GroupIds { get; set; }
    public string SpecificationName { get; set; }
    public string BeginTime { get; set; }
    public string ProductDate { get; set; }
    public string ManufacturerPlace { get; set; }
    public string TestMethod { get; set; }
    public string Result { get; set; }
    public string OrderNo { get; set; }
    public bool FinalRet { get; set; }
    public int ReportType { get; set; } = 1;
    public int ChemicalDataId { get; set; }
    public string Count { get; set; }
    public int CountInBox { get; set; }
    public string AuditUser { get; set; }
}