using System.Collections.Generic;
using Core.Dtos;
using Core.Dtos.Defect;

namespace Infrastructure.Services.Defect;

public interface IDefectService
{
    IEnumerable<DefectInfoDto> GetDefects(DefectQueryInfoDto dto, out int total);
    bool Edit(DefectInfoDto dto, out string message);
    IEnumerable<BaseOptionDto> GetOptions(bool sort = false);
}