namespace Core.Dtos.WrapQualityInspectionReport;

public class WrapQualityInspectionReportQueryInfoDto: BaseQueryInfoDto
{
    public string SpecificationId { get; set; }
    public string TurnId { get; set; }
    public string TeamId { get; set; }
    public string VolumePickUpId { get; set; }
    public string PackagingMachineId { get; set; }
    public string BeginTime { get; set; }
    public string EndTime { get; set; }
    public string Result { get; set; }
}