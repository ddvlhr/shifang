namespace Core.Dtos.Statistics;

public class FilterMeasureQualityInfoDto
{
    public string SpecificationName { get; set; }
    public string WeightQuality { get; set; }
    public string WeightQualityRate { get; set; }
    public string CircleQuality { get; set; }
    public string CircleQualityRate { get; set; }
    public string OvalQuality { get; set; }
    public string OvalQualityRate { get; set; }
    public string LengthQuality { get; set; }
    public string LengthQualityRate { get; set; }
    public string ResistanceQuality { get; set; }
    public string ResistanceQualityRate { get; set; }
    public string HardnessQuality { get; set; }
    public string HardnessQualityRate { get; set; }
}