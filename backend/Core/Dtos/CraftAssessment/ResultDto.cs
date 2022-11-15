using System.Collections.Generic;

namespace Core.Dtos.CraftAssessment;

public class ResultDto
{
    public string WorkShopName { get; set; }
    public int WorkShopRowCount { get; set; }
    public List<MachineModelItem> MachineModelList { get; set; }
    public List<double> FirstScore { get; set; }
    public double CraftScore { get; set; }

    public object TempList { get; set; }

    public class MachineModelItem
    {
        public string MachineModelName { get; set; }
        public int MachineModelRowCount { get; set; }
        public List<double> ThirdScoreList { get; set; }
        public double ThirdScoreMean { get; set; }
        public int ThirdScoreMeanRowCount { get; set; }
        public List<double> SecondScoreList { get; set; }
        public double SecondScoreMean { get; set; }
        public int SecondScoreMeanRowCount { get; set; }
    }
}