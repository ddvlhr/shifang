using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_indicator")]
public class Indicator : Entity
{
    [Column("name")] [MaxLength(64)] public string Name { get; set; }

    [Column("indicator_parent_id")] public int IndicatorParentId { get; set; }

    [ForeignKey(nameof(IndicatorParentId))]
    public IndicatorParent IndicatorParent { get; set; }

    [Column("indicator_project")] public IndicatorProject IndicatorProject { get; set; }

    [Column("unit")] public string Unit { get; set; }

    [Column("standard")] public string Standard { get; set; }
    
    [Column("score")]
    public double Score { get; set; }

    [Column("status")] public Status Status { get; set; }
}