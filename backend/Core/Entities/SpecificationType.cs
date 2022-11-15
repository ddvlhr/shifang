using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

/// <summary>
///     牌号类型
/// </summary>
[Table("t_specification_type")]
public class SpecificationType : BaseDataEntity
{
    [Column("remark")] [MaxLength(128)] public string Remark { get; set; }

    [Column("product_order_no")]
    [MaxLength(128)]
    public string ProductOrderNo { get; set; }

    [Column("inspection_order_no")]
    [MaxLength(128)]
    public string InspectionOrderNo { get; set; }

    [Column("material_order_no")]
    [MaxLength(128)]
    public string MaterialOrderNo { get; set; }

    [Column("factory_order_no")]
    [MaxLength(128)]
    public string FactoryOrderNo { get; set; }

    [Column("craft_order_no")]
    [MaxLength(128)]
    public string CraftOrderNo { get; set; }

    [Column("physical_order_no")] public string PhysicalOrderNo { get; set; }
}