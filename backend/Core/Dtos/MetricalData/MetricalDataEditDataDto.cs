namespace Core.Dtos.MetricalData;

public class MetricalDataEditDataDto
{
    public int GroupId { get; set; }
    public string DataInfo { get; set; }
}

public class DataInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}