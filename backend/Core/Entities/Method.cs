using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_method")]
public class Method : BaseDataEntity
{
    [Column("specification_types")] public string SpecificationTypes { get; set; }
}