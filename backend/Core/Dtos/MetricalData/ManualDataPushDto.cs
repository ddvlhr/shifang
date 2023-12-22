using System.Collections.Generic;

namespace Core.Dtos.MetricalData;

public class ManualDataPushDto
{
    public List<ManualDataPushTable> TableInfo { get; set; }
    public List<PieChartInfo> PieChartInfos { get; set; }
    public class ManualDataPushTable
    {
        public string SpecificationName { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Mean { get; set; }
        public double Sd { get; set; }
        public double Cpk { get; set; }
        public double Offset { get; set; }
        public int Total { get; set; }
        public int Quality { get; set; }
        public string QualityInfo { get; set; }
        public string Rate { get; set; }
    }

    public class PieChartInfo
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string ResistanceMean { get; set; }
        public string QualifiedRate { get; set; }
        public string GoodQualifiedRate { get; set; }
    }
}