using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_wrap_quality_inspection_report")]
[Comment("卷包质量检验报表")]
public class WrapQualityInspectionReport: Entity
{
    [Column("time")]
    [Comment("时间")]
    public DateTime Time { get; set; }
    [ForeignKey(nameof(SpecificationId))]
    public Specification Specification { get; set; }
    [Column("specification_id")]
    [Comment("牌号 Id")]
    public int SpecificationId { get; set; }
    [ForeignKey(nameof(TeamId))]
    public Team Team { get; set; }
    [Column("team_id")]
    [Comment("班组 Id")]
    public int TeamId { get; set; }
    [ForeignKey(nameof(TurnId))]
    public Turn Turn { get; set; }
    [Column("turn_id")]
    [Comment("班次 Id")]
    public int TurnId { get; set; }
    [ForeignKey(nameof(VolumePickUpId))]
    public VolumePickUp VolumePickUp { get; set; }
    [Column("volume_pick_up_id")]
    [Comment("卷接机 Id")]
    public int VolumePickUpId { get; set; }
    [ForeignKey(nameof(PackagingMachineId))]
    public PackagingMachine PackagingMachine { get; set; }
    [Column("packaging_machine_id")]
    [Comment("包装机 Id")]
    public int PackagingMachineId { get; set; }
    [Column("inspection_count")]
    [Comment("专检次数")]
    [MaxLength(64)]
    public string InspectionCount { get; set; }
    [Column("order_no")]
    [Comment("烟丝批号")]
    [MaxLength(64)]
    public string OrderNo { get; set; }
    [Column("inspector")]
    [Comment("检验员")]
    [MaxLength(64)]
    public string Inspector { get; set; }
    [Column("volume_pick_up_operator")]
    [Comment("卷接机操作工")]
    [MaxLength(64)]
    public string VolumePickUpOperator { get; set; }
    [Column("packaging_machine_operator")]
    [Comment("包装机操作工")]
    [MaxLength(64)]
    public string PackagingMachineOperator { get; set; }
    [Column("measure_group_ids", TypeName = "text")]
    [Comment("测量组数据 Id")]
    public string MeasureGroupIds { get; set; }
    [Column("total_points")]
    [Comment("总扣分")]
    public double TotalPoints { get; set; }
    [Column("volume_pick_up_points")]
    [Comment("卷接机扣分")]
    public double VolumePickUpPoints { get; set; }
    [Column("packaging_machine_points")]
    [Comment("包装机扣分")]
    public double PackagingMachinePoints { get; set; }
    [Column("result")]
    [Comment("质量等级")]
    public QualityResult Result { get; set; }
    [Column("batch_unqualified")]
    [Comment("批不合格项")]
    [MaxLength(256)]
    public string BatchUnqualified { get; set; }
    [Column("remark")]
    [Comment("备注")]
    [MaxLength(256)]
    public string Remark { get; set; }
}