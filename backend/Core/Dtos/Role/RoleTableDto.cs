using System.Collections.Generic;

namespace Core.Dtos.Role;

public class RoleTableDto : BaseTableDto
{
    public List<int> RoleMenu { get; set; }
    public bool CanSeeOtherData { get; set; }
}