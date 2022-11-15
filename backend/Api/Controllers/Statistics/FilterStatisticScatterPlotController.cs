using Core.Dtos.Statistics;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Statistics;

[Authorize]
public class FilterStatisticScatterPlotController : BaseController
{
    private readonly IFilterStandardDeviationStatisticService _fsdsService;

    public FilterStatisticScatterPlotController(IFilterStandardDeviationStatisticService fsdsService)
    {
        _fsdsService = fsdsService;
    }

    [HttpGet]
    [Route("filterStatisticScatterPlot")]
    public IActionResult Search([FromQuery] StatisticQueryInfoDto dto)
    {
        var data = _fsdsService.Search(dto);
        return Success(data);
    }
}