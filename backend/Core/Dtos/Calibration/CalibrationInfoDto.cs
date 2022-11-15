namespace Core.Dtos.Calibration;

public class CalibrationInfoDto
{
    public int Id { get; set; }
    public string Time { get; set; }
    public string Instance { get; set; }
    public string EquipmentTypeName { get; set; }
    public string Operation { get; set; }
    public string Unit { get; set; }
    public string UnitType { get; set; }
    public bool State { get; set; }
    public string Description { get; set; }
}