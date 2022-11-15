using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.MachineModel;

namespace Infrastructure.Services.BaseData;

public interface IMachineModelService
{
    IEnumerable<MachineModelTableDto> GetTable(MachineModelQueryInfoDto dto, out int total);
    bool Add(MachineModelEditDto dto, out string failReason);
    bool Update(MachineModelEditDto dto, out string failReason);
    IEnumerable<BaseOptionDto> GetOptions();
}