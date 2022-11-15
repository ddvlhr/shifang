using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_material_check_report")]
[Comment("物资申检报表")]
public class MaterialCheckReport: Entity
{
    [Column("test_date")]
    [Comment("检测日期")]
    public DateTime TestDate { get; set; }
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
    [ForeignKey(nameof(MachineId))]
    public Machine Machine { get; set; }
    [Column("machine_id")]
    [Comment("机台 Id")]
    public int MachineId { get; set; }
    [ForeignKey(nameof(MeasureTypeId))]
    public MeasureType MeasureType { get; set; }
    [Column("measure_type_id")]
    [Comment("测量类型 Id")]
    public int MeasureTypeId { get; set; }

    [Column("group_id")]
    [Comment("检测数据组数据 Id")]
    public int GroupId { get; set; }
    [Column("originator")]
    [Comment("发起人")]
    [MaxLength(64)]
    public string Originator { get; set; }
    [Column("operating_user")]
    [Comment("检验员")]
    [MaxLength(64)]
    public string OperatingUser { get; set; }
    [Column("qualified")]
    [Comment("合格状态")]
    public QualifiedStatus Qualified { get; set; }
    [Column("status")]
    [Comment("流程状态")]
    public MaterialCheckStatus Status { get; set; }
}