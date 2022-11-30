using System.Collections.Generic;

namespace Core.Dtos.ManualTestStatistic;

public class ManualTestStatisticInfoDto
{
    public List<TableItem> TableInfo { get; set; }
    public class TableItem
    {
        public string Result { get; set; }
        public int Count { get; set; }
    }
}