using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_material_template")]
public class MaterialTemplate : Entity
{
    [ForeignKey(nameof(SpecificationTypeId))]
    public SpecificationType SpecificationType { get; set; }

    [Column("specification_type_id")] public int SpecificationTypeId { get; set; }

    [Column("description", TypeName = "text")]
    public string Description { get; set; }
}