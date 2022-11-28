using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Specification;
using Core.Models;

namespace Infrastructure.Services.Specification;

public interface ISpecificationService
{
    IEnumerable<SpecificationTableDto> Get(SpecificationQueryInfoDto dto, out int total);
    bool Add(SpecificationEditDto dto, out string failReason);
    bool Update(SpecificationEditDto dto, out string failReason);
    SpecificationEditDto GetInfo(int id);
    IEnumerable<BaseOptionDto> GetOptions();
    IEnumerable<BaseOptionDto> GetSpecificationsByTypeId(int id);
    bool GetIndicatorsTableDescById(int id, out TableEditor result, out string message, int measureTypeId = 0, int machineModelId = 0);
}