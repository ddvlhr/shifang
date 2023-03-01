using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Permission;

namespace Infrastructure.Services.System;

public interface IPermissionService
{
    IEnumerable<PermissionTreeDto> GetPermissionTree(int role);
    bool Edit(EditPermissionDto dto, out string failReason);
    bool Remove(List<int> ids, out string message);
    IEnumerable<BaseTreeDto> GetAllPermissionTree();
    IEnumerable<BaseOptionDto> GetOptions(bool root);
}