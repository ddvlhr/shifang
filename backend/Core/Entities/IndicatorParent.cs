using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_indicator_parent")]
public class IndicatorParent : Entity
{
    [Column("name")] public string Name { get; set; }

    [Column("status")] public Status Status { get; set; }
}