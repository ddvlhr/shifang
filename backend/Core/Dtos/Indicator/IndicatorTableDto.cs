namespace Core.Dtos.Indicator;

public class IndicatorTableDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Parent { get; set; }
    public string ParentName { get; set; }
    public int Project { get; set; }
    public string Unit { get; set; }
    public string Standard { get; set; }
    public bool State { get; set; }
}