using System.Collections.Generic;

namespace Core.Dtos.Report;

public class EditReportAppearanceDto
{
    public int ReportId { get; set; }
    public int Total { get; set; }
    public int FinalRet { get; set; }
    public IEnumerable<AppearanceInfo> Appearances { get; set; }

    public class AppearanceInfo
    {
        public int DbId { get; set; }
        public int IndicatorId { get; set; }
        public string Frequency { get; set; }
        public string SubScore { get; set; }
    }
}