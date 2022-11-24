using System.Collections.Generic;
using Core.Dtos.Defect;

namespace Core.Dtos.WrapProcessInspectionReport;

public class WrapProcessInspectionReportInfoDto
{
    public int Id { get; set; }
    public string Time { get; set; }
    public int SpecificationId { get; set; }
    public string SpecificationName { get; set; }
    public int TurnId { get; set; }
    public string TurnName { get; set; }
    public int MachineId { get; set; }
    public string MachineName { get; set; }
    public string BatchNumber { get; set; }
    public string OperatorName { get; set; }
    public string Inspector { get; set; }
    public int Result { get; set; }
    public double Score { get; set; }

    public int WeightUpper { get; set; }
    public int WeightLower { get; set; }
    public int ResistanceUpper { get; set; }
    public int ResistanceLower { get; set; }
    public string OtherIndicators { get; set; }
    public int OtherCount { get; set; }

    public List<BaseDefectInfoDto> Defects { get; set; }
    
    public string BatchUnqualified { get; set; }

    public string Remark { get; set; }
}