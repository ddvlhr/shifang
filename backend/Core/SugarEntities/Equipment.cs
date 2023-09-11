using System;
using Core.Entities;
using Core.Enums;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_equipment")]
public class Equipment
{
    [SugarColumn(IsIgnore = true)]
    public string ConnectionId { get; set; }
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    [SugarColumn(ColumnName = "name", Length = 64)]
    public string Name { get; set; }
    [SugarColumn(ColumnName = "description", Length = 256)]
    public string Description { get; set; }
    [SugarColumn(ColumnName = "instance", Length = 64)]
    public string Instance { get; set; }
    [SugarColumn(ColumnName = "ip", Length = 64)]
    public string Ip { get; set; }
    [SugarColumn(ColumnName = "version", Length = 64)]
    public string Version { get; set; }
    [SugarColumn(ColumnName = "equipment_type")]
    public EquipmentType EquipmentType { get; set; }
    [SugarColumn(ColumnName = "status")]
    public OnlineStatus Status { get; set; }

    [SugarColumn(ColumnName = "last_connect_time")]
    public DateTime LastConnectTime { get; set; }
}