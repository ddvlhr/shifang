using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos.Statistics;

namespace Infrastructure.Services.Statistics
{
    public interface IOriginDataStatisticService
    {
        IEnumerable<OriginDataStatisticDto.OriginDataStatisticDataDto> Search(OriginDataStatisticDto.OriginDataStatisticQueryDto dto);
    }
}
