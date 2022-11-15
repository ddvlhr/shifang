namespace Core.Dtos.MetricalData;

public class WaterRecordTableDto
{
    public int Id { get; set; }
    public string TestDateTime { get; set; }
    public string Before { get; set; }
    public string After { get; set; }
    public string Water { get; set; }
}