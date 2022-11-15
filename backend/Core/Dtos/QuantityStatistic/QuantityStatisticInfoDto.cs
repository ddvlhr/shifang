namespace Core.Dtos.QuantityStatistic;

public class QuantityStatisticInfoDto
{
    public string SpecificationName { get; set; }
    public string Max { get; set; }
    public string Min { get; set; }
    public string Mean { get; set; }
    public string MeanOffset { get; set; }
    public string Sd { get; set; }
    public string Cpk { get; set; }
    public string Offs { get; set; }
    public string Cv { get; set; }
    public string Quality { get; set; }
    public string Rate { get; set; }
}