using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public abstract class BaseReportAppearance : Entity
{
    [Column("report_id")] public int ReportId { get; set; }

    [Column("indicator_id")] public int IndicatorId { get; set; }

    [ForeignKey(nameof(IndicatorId))] public Indicator Indicator { get; set; }

    [Column("frequency")] public int Frequency { get; set; }

    [Column("sub_score")] public int SubScore { get; set; }
}