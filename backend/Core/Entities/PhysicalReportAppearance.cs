using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_physical_report_appearance")]
public class PhysicalReportAppearance : BaseReportAppearance
{
    [ForeignKey(nameof(ReportId))] public PhysicalReport PhysicalReport { get; set; }
}