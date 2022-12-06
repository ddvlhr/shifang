namespace Core.Models;

public class ShiFangSettings
{
    public string Version { get; set; }
    public bool EnableErrorPush { get; set; }
    public string[] ErrorPushAt { get; set; }
    public string DataSource { get; set; }
    public int AdminTypeId { get; set; }
}