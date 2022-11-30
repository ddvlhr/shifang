using Core.Dtos.ManualInspectionReport;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Statistics;

[Authorize]
public class ManualTestStatisticController: BaseController
{
    private readonly IManualTestStatisticService _manualTestStatisticService;

    public ManualTestStatisticController(
        IManualTestStatisticService manualTestStatisticService)
    {
        _manualTestStatisticService = manualTestStatisticService;
    }

    [HttpGet("statistic/manualTest")]
    public IActionResult Search([FromQuery] ManualInspectionReportQueryInfoDto dto)
    {
        return Success(_manualTestStatisticService.Search(dto));
    }
}