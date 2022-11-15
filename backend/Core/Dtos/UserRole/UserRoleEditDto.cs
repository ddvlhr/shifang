using System.Collections.Generic;

namespace Core.Dtos.UserRole;

public class UserRoleEditDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public List<int> Users { get; set; }
}