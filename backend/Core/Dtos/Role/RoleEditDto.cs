using System.Collections.Generic;

namespace Core.Dtos.Role;

public class RoleEditDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<int> RoleMenu { get; set; }
    public bool CanSeeOtherData { get; set; }
    public bool State { get; set; }
}