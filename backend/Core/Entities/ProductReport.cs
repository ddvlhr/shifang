using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_product_report")]
public class ProductReport : BaseReport
{
    [Column("batch_number")]
    [MaxLength(64)]
    [Comment("烟支批号")]
    public string BatchNumber { get; set; }
    [Column("end_tribe_count")]
    [Comment("端部落丝量")]
    public double EndTribeCount { get; set; }
    [Column("flame_out_count")]
    [Comment("熄火支数")]
    public int FlameOutCount { get; set; }
    [Column("inspector")]
    [MaxLength(64)]
    [Comment("检验员")]
    public string Inspector { get; set; }
    [Column("moisture_rate")]
    [Comment("含末率")]
    public double MoistureRate { get; set; }
}