using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Role;

namespace Infrastructure.Services.System;

public interface IRoleService
{
    IEnumerable<RoleTableDto> GetTable(BaseQueryInfoDto dto, out int total);
    bool Add(RoleEditDto dto, out string failReason);
    bool Update(RoleEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions();
}