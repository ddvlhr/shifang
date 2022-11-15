using System.Collections.Generic;
using System.IO;
using Core.Dtos.CraftAssessment;

namespace Infrastructure.Services.Statistics;

public interface ICraftAssessmentService
{
    IEnumerable<TableResultDto> Search(QueryInfoDto dto);
    MemoryStream Download(QueryInfoDto dto);
}