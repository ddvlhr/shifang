using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_manual_inspection_report_defect")]
[Comment("手工检验报表缺陷项")]
public class ManualInspectionReportDefect: Entity
{
    [ForeignKey(nameof(ReportId))]
    public ManualInspectionReport ManualInspectionReport { get; set; }
    [Column("report_id")]
    [Comment("手工检验报表 Id")]
    public int ReportId { get; set; }
    [ForeignKey(nameof(DefectId))]
    public Defect Defect { get; set; }
    [Column("defect_id")]
    [Comment("缺陷 Id")]
    public int DefectId { get; set; }
    [Column("count")]
    [Comment("数量")]
    public int Count { get; set; }
}