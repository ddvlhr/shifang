using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_craft_indicator_rule")]
public class CraftIndicatorRule : Entity
{
    [Column("model_id")] public int ModelId { get; set; }

    [ForeignKey(nameof(ModelId))] public Model Model { get; set; }

    [Column("rules", TypeName = "text")] public string Rules { get; set; }
}