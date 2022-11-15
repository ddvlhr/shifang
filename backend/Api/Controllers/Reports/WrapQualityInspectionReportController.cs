using System.Collections.Generic;
using Core.Dtos.WrapQualityInspectionReport;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

[Authorize]
public class WrapQualityInspectionReportController: BaseController
{
    private readonly IWrapQualityInspectionReportService _wiService;

    public WrapQualityInspectionReportController(IWrapQualityInspectionReportService wiService)
    {
        _wiService = wiService;
    }

    [HttpGet, Route("report/wrapQuality")]
    public IActionResult GetReports([FromQuery] WrapQualityInspectionReportQueryInfoDto dto)
    {
        var list = _wiService.GetReport(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("report/wrapQuality")]
    public IActionResult EditReport([FromBody] WrapQualityInspectionReportInfoDto dto)
    {
        return _wiService.EditReport(dto, out var message) ? Success(msg: message) : Error(message);
    }

    [HttpDelete, Route("report/wrapQuality")]
    public IActionResult RemoveReports([FromBody] List<int> ids)
    {
        return _wiService.RemoveReport(ids, out var message) ? Success(msg: message) : Error(message);
    }
}