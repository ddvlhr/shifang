using System;
using System.Collections.Generic;
using Core.Enums;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_group")]
public class MetricalGroup: SugarEntity
{
    [SugarColumn(ColumnName = "begin_time")] 
    public DateTime BeginTime { get; set; }
    [SugarColumn(ColumnName = "end_time")] 
    public DateTime EndTime { get; set; }
    [SugarColumn(ColumnName = "production_time", IsNullable = true)] 
    public DateTime? ProductionTime { get; set; }
    [SugarColumn(ColumnName = "deliver_time", IsNullable = true)] 
    public DateTime? DeliverTime { get; set; }
    [SugarColumn(ColumnName = "specification_id")] 
    public int SpecificationId { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(SpecificationId))]
    public Specification Specification { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(TeamId))]
    public Team Team { get; set; }
    [SugarColumn(ColumnName = "team_id", IsNullable = true)]
    public int TeamId { get; set; }
    [SugarColumn(ColumnName = "turn_id", IsNullable = true)]
    public int TurnId { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(TurnId))]
    public Turn Turn { get; set; }
    [SugarColumn(ColumnName = "machine_id", IsNullable = true)]
    public int MachineId { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(MachineId))]
    public Machine Machine { get; set; }
    [SugarColumn(ColumnName = "measure_type_id", IsNullable = true)]
    public int MeasureTypeId { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(MeasureTypeId))]
    public MeasureType MeasureType { get; set; }
    [SugarColumn(ColumnName = "order_no", Length = 128)]
    public string OrderNo { get; set; }
    [SugarColumn(ColumnName = "instance", Length = 64)]
    public string Instance { get; set; }
    [SugarColumn(ColumnName = "pickup_way")]
    public int PickUpWay { get; set; }
    [SugarColumn(ColumnName = "user_name", Length = 64)]
    public string UserName { get; set; }
    [SugarColumn(ColumnName = "count")]
    public int Count { get; set; }

    [SugarColumn(ColumnName = "user_data", Length = 255)]
    public string UserData { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(MetricalData.GroupId))]
    public List<MetricalData> DataList { get; set; }

    [SugarColumn(ColumnName = "equipment_type")]
    public EquipmentType EquipmentType { get; set; } = EquipmentType.Rt;
}