using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_factory_site")]
public class FactorySite : BaseDataEntity
{
    [Column("specification_types")] public string SpecificationTypes { get; set; }
}