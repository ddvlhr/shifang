using System.Collections.Generic;

namespace Core.Dtos.WrapQualityInspectionReport;

public class WrapQualityInspectionReportInfoDto
{
    public int Id { get; set; }
    public string Time { get; set; }
    public int SpecificationId { get; set; }
    public string SpecificationName { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public int TurnId { get; set; }
    public string TurnName { get; set; }
    public int VolumePickUpId { get; set; }
    public string VolumePickUpName { get; set; }
    public int PackagingMachineId { get; set; }
    public string PackagingMachineName { get; set; }
    public string Count { get; set; }
    public string OrderNo { get; set; }
    public string Inspector { get; set; }
    public string VolumePickUpOperator { get; set; }
    public string PackagingMachineOperator { get; set; }
    public int Result { get; set; }

    public List<BaseDefectInfoDto> Defects { get; set; }
}