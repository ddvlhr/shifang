using Core.Dtos.Statistics;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Statistics
{
    [Authorize]
    public class OriginDataStatisticController : BaseController
    {
        private readonly IOriginDataStatisticService _odService;

        public OriginDataStatisticController(IOriginDataStatisticService odService)
        {
            _odService = odService;
        }

        [HttpPost("statistic/originDataStatistic")]
        public IActionResult Search([FromBody] OriginDataStatisticDto.OriginDataStatisticQueryDto dto)
        {
            return Success(_odService.Search(dto));
        }
    }
}
