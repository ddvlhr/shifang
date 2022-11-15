namespace Core.Dtos.CraftAssessment;

public class TableResultDto
{
    public string WorkShopName { get; set; }
    public int WorkShopNameCount { get; set; } = 0;
    public string MachineModelName { get; set; }
    public int MachineModelRowCount { get; set; } = 0;
    public double ThirdScore { get; set; }
    public int ThirdScoreRowCount { get; set; } = 0;
    public double ThirdMeanScore { get; set; }
    public int ThirdMeanScoreRowCount { get; set; } = 0;
    public double SecondScore { get; set; }
    public int SecondScoreRowCount { get; set; } = 0;
    public double SecondMeanScore { get; set; }
    public int SecondMeanScoreRowCount { get; set; } = 0;
    public double FirstScore { get; set; }
    public int FirstScoreRowCount { get; set; } = 0;
    public double CraftScore { get; set; }
    public int CraftScoreRowCount { get; set; } = 0;
}