using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_user_role")]
public class UserRole : Entity
{
    [ForeignKey(nameof(RoleId))] public Role Role { get; set; }

    [Column("role_id")] public int RoleId { get; set; }

    [Column("user_ids", TypeName = "text")]
    public string UserIds { get; set; }
}