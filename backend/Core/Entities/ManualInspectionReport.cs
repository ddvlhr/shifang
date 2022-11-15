using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_manual_inspection_report")]
public class ManualInspectionReport: Entity
{
    [Column("time")]
    [Comment("时间")]
    public DateTime Time { get; set; }
    [ForeignKey(nameof(SpecificationId))]
    public Specification Specification { get; set; }
    [Column("specification_id")]
    [Comment("牌号 Id")]
    public int SpecificationId { get; set; }
    [Column("operation")]
    [MaxLength(64)]
    [Comment("操作工")]
    public string Operation { get; set; }
    [Column("count")]
    [Comment("数量")]
    public int Count { get; set; }

    public virtual List<ManualInspectionReportDefect> Defects { get; set; }
    [Column("result")]
    [Comment("判定结果")]
    public QualityResult Result { get; set; }
}