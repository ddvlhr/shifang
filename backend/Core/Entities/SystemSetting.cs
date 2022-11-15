using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_system_setting")]
public class SystemSetting : Entity
{
    [Column("can_see_other_data")] public bool CanSeeOtherData { get; set; }

    [Column("mysql_server_name")] public string MySqlServerName { get; set; }

    [Column("admin_id")] public int AdminId { get; set; }

    [Column("physical_type_id")] public int PhysicalTypeId { get; set; }

    [Column("inspection_type_id")] public int InspectionTypeId { get; set; }

    [Column("production_type_id")] public int ProductionTypeId { get; set; }

    [Column("material_type_id")] public int MaterialTypeId { get; set; }

    [Column("factory_type_id")] public int FactoryTypeId { get; set; }

    [Column("chemical_type_id")] public int ChemicalTypeId { get; set; }

    [Column("craft_type_id")] public int CraftTypeId { get; set; }

    [Column("filter_type_id")] public int FilterTypeId { get; set; }

    [Column("weight")] public int Weight { get; set; }

    [Column("circle")] public int Circle { get; set; }

    [Column("oval")] public int Oval { get; set; }

    [Column("length")] public int Length { get; set; }

    [Column("resistance")] public int Resistance { get; set; }

    [Column("hardness")] public int Hardness { get; set; }

    [Column("recycle_box_type_id")] public string RecycleBoxTypeId { get; set; }

    [Column("box_making_work_shop_id")] public int BoxMakingWorkShopId { get; set; }
}