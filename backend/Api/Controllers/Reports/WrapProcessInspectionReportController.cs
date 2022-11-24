using System.Collections.Generic;
using Core.Dtos.WrapProcessInspectionReport;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

[Authorize]
public class WrapProcessInspectionReportController: BaseController
{
    private readonly IWrapProcessInspectionReportService _wpService;

    public WrapProcessInspectionReportController(IWrapProcessInspectionReportService wpService)
    {
        _wpService = wpService;
    }

    [HttpGet]
    [Route("report/wrapProcess")]
    public IActionResult GetReports([FromQuery] WrapProcessInspectionReportQueryInfoDto dto)
    {
        var list = _wpService.GetReports(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("report/wrapProcess")]
    public IActionResult EditReport([FromBody] WrapProcessInspectionReportInfoDto dto)
    {
        return _wpService.EditReport(dto, out var message) ? Success(msg: message) : Error(message);
    }
    
    [HttpDelete]
    [Route("report/wrapProcess")]
    public IActionResult DeleteReport([FromBody] List<int> ids)
    {
        return _wpService.RemoveReports(ids, out var message) ? Success(msg: message) : Error(message);
    }
}