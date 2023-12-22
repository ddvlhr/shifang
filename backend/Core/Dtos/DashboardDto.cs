using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Dtos.MetricalData.ManualDataPushDto;

namespace Core.Dtos
{
    public class DashboardDto
    {
        public class ManualCheckerInfo
        {
            public string No { get; set; }
            public int Count { get; set; }
            public int QualityCount { get; set; }
            public int MoreCount { get; set; }
            public int LessCount { get; set; }
            public int GoodCount { get; set; }
            public string QualifiedRate { get; set; }
            public string GoodRate { get; set; }
        }

        public class ManualQueryInfoDto : BaseQueryInfoDto
        {
            public List<int> SpecificationId { get; set; }
            public List<int> TurnIds { get; set; }
            public string WorkShop { get; set; }
        }

        public class ManualSummaryInfoDto
        {
            public List<ManualDataPushTable> TableInfo { get; set; }
            public List<PieChartInfo> PieChartInfo { get; set; }
            public List<ManualCheckerInfo> CheckerInfo { get; set; }
        }
    }
}
