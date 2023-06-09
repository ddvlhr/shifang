using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Entities;

[Table("t_role")]
public class Role : Entity
{
    [Column("name")] [MaxLength(64)] public string Name { get; set; }

    [Column("role_menu", TypeName = "text")]
    public string RoleMenu { get; set; }

    [Column("can_see_other_data")] public bool CanSeeOtherData { get; set; }

    [Column("status")] public Status Status { get; set; }

    [Column("equipment_type")]
    public DepartmentType EquipmentType { get; set; }
}