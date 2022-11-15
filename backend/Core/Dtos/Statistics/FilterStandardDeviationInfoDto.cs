using System.Collections.Generic;

namespace Core.Dtos.Statistics;

public class FilterStandardDeviationInfoDto
{
    public string SpecificationName { get; set; }
    public List<string> Ids { get; set; }
    public List<SpecificationStatisticInfo> Weight { get; set; }
    public string WeightMean { get; set; }
    public List<SpecificationStatisticInfo> Circle { get; set; }
    public string CircleMean { get; set; }
    public List<SpecificationStatisticInfo> Oval { get; set; }
    public string OvalMean { get; set; }
    public List<SpecificationStatisticInfo> Length { get; set; }
    public string LengthMean { get; set; }
    public List<SpecificationStatisticInfo> Resistance { get; set; }
    public string ResistanceMean { get; set; }
    public List<SpecificationStatisticInfo> Hardness { get; set; }
    public string HardnessMean { get; set; }

    public class DataInfo
    {
        public int GroupId { get; set; }
        public double Value { get; set; }
    }

    public class SpecificationStatisticInfo
    {
        public string SpecificationName { get; set; }
        public List<string[]> data { get; set; }
    }
}