using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_product_report_defect")]
public class ProductReportDefect: Entity
{
    [ForeignKey(nameof(ReportId))]
    public ProductReport ProductReport { get; set; }
    [Column("report_id")]
    [Comment("成品质量检验报表 Id")]
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