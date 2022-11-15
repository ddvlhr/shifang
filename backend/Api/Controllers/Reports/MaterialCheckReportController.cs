using System.Collections.Generic;
using Core.Dtos.MaterialCheckReport;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Api.Controllers.Reports;

[Authorize]
public class MaterialCheckReportController: BaseController
{
    private readonly IMaterialCheckReportService _mcService;

    public MaterialCheckReportController(IMaterialCheckReportService mcService)
    {
        _mcService = mcService;
    }

    [HttpGet, Route("report/materialCheck")]
    public IActionResult GetReports([FromQuery] MaterialCheckReportQueryInfoDto dto)
    {
        var list = _mcService.GetReports(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("report/materialCheck")]
    public IActionResult EditReport([FromBody] MaterialCheckReportInfoDto dto)
    {
        return _mcService.EditReport(dto, out var message) ? Success() : Error();
    }

    [HttpDelete, Route("report/materialCheck")]
    public IActionResult RemoveReports([FromBody] List<int> ids)
    {
        return _mcService.RemoveReports(ids, out var message) ? Success(msg: message) : Error(message);
    }
}