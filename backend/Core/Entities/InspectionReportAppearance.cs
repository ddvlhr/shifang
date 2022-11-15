using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_inspection_report_appearance")]
public class InspectionReportAppearance : BaseReportAppearance
{
    [Column("data_id")] public int DataId { get; set; }
}