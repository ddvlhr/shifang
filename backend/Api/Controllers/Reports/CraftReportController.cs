using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

[Authorize]
public class CraftReportController : BaseController
{
    private readonly ICraftReportService _crService;

    public CraftReportController(ICraftReportService crService)
    {
        _crService = crService;
    }

    [HttpGet]
    [Route("craftReport")]
    public IActionResult GetTable([FromQuery] QueryInfoDto dto)
    {
        var list = _crService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpGet]
    [Route("craftReport/{id}")]
    public IActionResult GetInfo(int id)
    {
        var dto = _crService.GetInfo(id);
        return Success(dto);
    }

    [HttpPost]
    [Route("craftReport")]
    public IActionResult Edit([FromBody] CraftReportEditDto dto)
    {
        return _crService.Edit(dto) ? Success() : Error();
    }

    [HttpGet]
    [Route("craftReport/{id}/info")]
    public IActionResult GetReportInfo(int id)
    {
        return Success(_crService.GetReportInfo(id));
    }
}