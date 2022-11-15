using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_water_record")]
public class WaterRecord : Entity
{
    [ForeignKey(nameof(GroupId))] public Group Group { get; set; }

    [Column("group_id")] public int GroupId { get; set; }

    [Column("before")] public double Before { get; set; }

    [Column("after")] public double After { get; set; }

    [Column("water")] public double Water { get; set; }

    [Column("data_id")] public int DataId { get; set; }

    [Column("data_test_time")] public DateTime DataTestTime { get; set; }
}