namespace Core.Dtos.Indicator;

public class IndicatorEditDto : BaseEditDto
{
    public int Parent { get; set; }
    public string Unit { get; set; }
    public string Standard { get; set; }
    public int Project { get; set; }
}