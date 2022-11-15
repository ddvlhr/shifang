using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_defect")]
[Comment("缺陷表")]
public class Defect: BaseDataEntity
{
    [ForeignKey(nameof(DefectTypeId))]
    public DefectType DefectType { get; set; }
    [Column("defect_type_id")]
    [Comment("缺陷类别 Id")]
    public int DefectTypeId { get; set; }
    [Column("defect_events_id")]
    [Comment("缺陷类别小项 Id")]
    public int DefectEventsId { get; set; }
    [Column("defect_short_name")]
    [MaxLength(256)]
    [Comment("缺陷简称")]
    public string DefectShortName { get; set; }
    [Column("defect_code")]
    [MaxLength(64)]
    [Comment("缺陷代码")]
    public string DefectCode { get; set; }
    [Column("description", TypeName = "text")]
    [Comment("缺陷描述")]
    public string Description { get; set; }
    [Column("defect_category")]
    [Comment("缺陷分类[A, B, C, D]")]
    public DefectCategory DefectCategory { get; set; }
    [Column("score")]
    [Comment("扣分值")]
    public double Score { get; set; }
}