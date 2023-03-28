namespace Core.Models;

public class ShiFangSettings
{
    public string Version { get; set; }
    public bool EnableErrorPush { get; set; }
    public string[] ErrorPushAt { get; set; }
    public string DataSource { get; set; }
    public int AdminTypeId { get; set; }
    public int Weight { get; set; }
    public int Circle { get; set; }
    public int Oval { get; set; }
    public int Length { get; set; }
    public int Resistance { get; set; }
    public int Hardness { get; set; }
    public int CigarTypeId { get; set; }
    public DataFormatDecimal IndicatorDecimal { get; set; }
}