using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_specification_type_rule")]
public class SpecificationTypeRule : Entity
{
    [Column("specification_type_id")] public int SpecificationTypeId { get; set; }

    [ForeignKey(nameof(SpecificationTypeId))]
    public SpecificationType SpecificationType { get; set; }

    [Column("rules", TypeName = "text")] public string Rules { get; set; }
}