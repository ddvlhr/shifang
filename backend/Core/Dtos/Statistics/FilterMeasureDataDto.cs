using System.Collections.Generic;

namespace Core.Dtos.Statistics;

public class FilterMeasureDataDto
{
    public List<double> WeightList { get; set; }
    public List<double> CircleList { get; set; }
    public List<double> OvalList { get; set; }
    public List<double> LengthList { get; set; }
    public List<double> ResistanceList { get; set; }
    public List<double> HardnessList { get; set; }
}