using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_month_craft_report")]
public class MonthCraftReport : Entity
{
    [Column("part_name")] [MaxLength(128)] public string PartName { get; set; }

    [Column("time")] public DateTime Time { get; set; }

    [Column("user")] [MaxLength(64)] public string User { get; set; }

    [Column("score")] public double Score { get; set; }

    [Column("result")] public string Result { get; set; }
}