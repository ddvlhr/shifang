using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Function;

namespace Infrastructure.Services.System;

public interface IFunctionService
{
    bool Add(EditFunctionDto dto, out string failReason);
    IEnumerable<FunctionTableDto> GetMenuFunctions(int id);
    EditFunctionDto GetFunction(int id);
    bool Update(EditFunctionDto dto, out string failReason);
    bool Delete(int id, out string failReason);
    IEnumerable<BaseTreeDto> GetMenuFunctions();
}