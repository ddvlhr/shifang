using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public abstract class BaseGroup : Entity
{
    [Column("begin_time")] public DateTime BeginTime { get; set; }

    [Column("end_time")] public DateTime EndTime { get; set; }

    [Column("production_time")] public DateTime? ProductionTime { get; set; }

    [Column("deliver_time")] public DateTime? DeliverTime { get; set; }

    [Column("specification_id")] public int SpecificationId { get; set; }

    [ForeignKey(nameof(SpecificationId))] public Specification Specification { get; set; }
    [ForeignKey(nameof(TeamId))]
    public Team Team { get; set; }
    [Column("team_id")]
    public int TeamId { get; set; }

    [Column("turn_id")] public int TurnId { get; set; }

    public virtual Turn Turn { get; set; }

    [Column("machine_id")] public int MachineId { get; set; }
    
    public virtual Machine Machine { get; set; }

    [Column("measure_type_id")] public int MeasureTypeId { get; set; }

    [ForeignKey(nameof(MeasureTypeId))] public MeasureType MeasureType { get; set; }

    public virtual MachineModel MachineModel { get; set; }

    [Column("machine_model_id")] public int MachineModelId { get; set; }

    [Column("order_no")] [MaxLength(128)] public string OrderNo { get; set; }

    [Column("instance")] [MaxLength(64)] public string Instance { get; set; }

    [Column("pickup_way")] public PickUpWay PickUpWay { get; set; }

    [Column("user_id")] public int UserId { get; set; }

    [Column("user_name")] public string UserName { get; set; }

    [Column("from_records", TypeName = "text")]
    public string FromRecords { get; set; }
    
    [Column("count")]
    public int Count { get; set; }
}

public enum PickUpWay
{
    Manual
}