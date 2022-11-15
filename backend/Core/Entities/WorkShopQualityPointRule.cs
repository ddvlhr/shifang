using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_work_shop_quality_point_rule")]
public class WorkShopQualityPointRule : Entity
{
    [Column("work_shop_id")] public int WorkShopId { get; set; }

    [ForeignKey(nameof(WorkShopId))] public WorkShop WorkShop { get; set; }

    [Column("specification_type_id")] public int SpecificationTypeId { get; set; }

    [ForeignKey(nameof(SpecificationTypeId))]
    public SpecificationType SpecificationType { get; set; }

    [Column("physical_points_percent")] public int PhysicalPointsPercent { get; set; }

    [Column("physical_all_percent")] public int PhysicalAllPercent { get; set; }

    [Column("inspection_points_percent")] public int InspectionPointsPercent { get; set; }

    [Column("inspection_all_percent")] public int InspectionAllPercent { get; set; }

    [Column("product_points_percent")] public int ProductPointsPercent { get; set; }

    [Column("product_all_percent")] public int ProductAllPercent { get; set; }

    [Column("status")] public Status Status { get; set; }
}