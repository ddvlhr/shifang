using System.Collections.Generic;
using Core.Dtos.ManualInspectionReport;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

[Authorize]
public class ManualInspectionReportController : BaseController
{
    private readonly IManualInspectionReportService _miService;

    public ManualInspectionReportController(IManualInspectionReportService miService)
    {
        _miService = miService;
    }

    [HttpGet, Route("report/manualInspection")]
    public IActionResult GetReports([FromQuery] ManualInspectionReportQueryInfoDto dto)
    {
        var list = _miService.GetReports(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("report/manualInspection")]
    public IActionResult EditReport([FromBody] ManualInspectionReportInfoDto dto)
    {
        return _miService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    [HttpDelete, Route("report/manualInspection")]
    public IActionResult DeleteReports([FromBody] List<int> ids)
    {
        return _miService.Remove(ids, out var message) ? Success(msg: message) : Error(message);
    }
}