using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_wrap_process_inspection_report")]
public class WrapProcessInspectionReport: Entity
{
    [Column("time")]
    [Comment("检测时间")]
    public DateTime Time { get; set; }
    [ForeignKey(nameof(SpecificationId))]
    public Specification Specification { get; set; }
    [Column("specification_id")]
    [Comment("牌号ID")]
    public int SpecificationId { get; set; }
    [ForeignKey(nameof(TurnId))]
    public Turn Turn { get; set; }
    [Column("turn_id")]
    [Comment("班次ID")]
    public int TurnId { get; set; }
    [ForeignKey(nameof(MachineId))]
    public Machine Machine { get; set; }
    [Column("machine_id")]
    [Comment("机台ID")]
    public int MachineId { get; set; }
    [Column("batch_number")]
    [MaxLength(64)]
    [Comment("批号")]
    public string BatchNumber { get; set; }
    [Column("operator_name")]
    [MaxLength(64)]
    [Comment("操作员")]
    public string OperatorName { get; set; }
    [Column("score")]
    [Comment("扣分")]
    public double Score { get; set; }
    [Column("result")]
    [Comment("判定结果")]
    public QualityResult Result { get; set; }
    [Column("inspector")]
    [Comment("检验员")]
    [MaxLength(64)]
    public string Inspector { get; set; }
    [Column("weight_upper")]
    [Comment("重量偏大支数")]
    public int WeightUpper { get; set; }
    [Column("weight_lower")]
    [Comment("重量偏小支数")]
    public int WeightLower { get; set; }
    [Column("resistance_upper")]
    [Comment("吸阻偏大支数")]
    public int ResistanceUpper { get; set; }
    [Column("resistance_lower")]
    [Comment("吸阻偏小支数")]
    public int ResistanceLower { get; set; }
    [Column("other_indicators")]
    [Comment("其他指标超标")]
    [MaxLength(512)]
    public string OtherIndicators { get; set; }
    [Column("other_count")]
    [Comment("其他指标超标支数")]
    public int OtherCount { get; set; }
    [Column("batch_unqualified")]
    [Comment("批不合格项")]
    [MaxLength(256)]
    public string BatchUnqualified { get; set; }
    [Column("remark")]
    [MaxLength(256)]
    [Comment("备注")]
    public string Remark { get; set; }
}