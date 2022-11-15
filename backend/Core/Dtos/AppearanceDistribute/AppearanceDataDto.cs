namespace Core.Dtos.AppearanceDistribute;

public class AppearanceDataDto
{
    public int GroupId { get; set; }
    public int Count { get; set; }
    public int SpecificationId { get; set; }
    public string SpecificationName { get; set; }
    public int TurnId { get; set; }
    public string TurnName { get; set; }
    public int MachineModelId { get; set; }
    public string MachineModelName { get; set; }
    public int AppearanceParentId { get; set; }
    public string AppearanceParentName { get; set; }
    public int AppearanceId { get; set; }
    public string AppearanceName { get; set; }
    public double SubScore { get; set; }
    public int Frequency { get; set; }
}