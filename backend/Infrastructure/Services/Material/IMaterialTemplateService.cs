using System.Collections.Generic;
using Core.Dtos.Material;

namespace Infrastructure.Services.Material;

public interface IMaterialTemplateService
{
    IEnumerable<MaterialTemplateTableDto> GetTable(MaterialTemplateQueryInfoDto dto, out int total);
    MaterialTemplateEditDto Get(int typeId);
    bool Update(MaterialTemplateEditDto dto, out string failReason);
}