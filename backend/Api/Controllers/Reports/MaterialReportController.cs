using Core.Dtos.Material;
using Infrastructure.Services.MetricalData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

[Authorize]
public class MaterialReportController : BaseController
{
    private readonly IMaterialReportService _mrService;

    public MaterialReportController(IMaterialReportService mrService)
    {
        _mrService = mrService;
    }

    [HttpGet]
    [Route("materialReport")]
    public IActionResult GetTable([FromQuery] MaterialReportQueryInfoDto dto)
    {
        var list = _mrService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpGet]
    [Route("materialReport/template/{id}")]
    public IActionResult GetTemplate(int id)
    {
        var desc = _mrService.GetTemplate(id);
        return Success(desc);
    }

    [HttpGet]
    [Route("materialReport/{id}")]
    public IActionResult GetInfo(int id)
    {
        var data = _mrService.GetInfo(id);
        return Success(data);
    }

    [HttpPost]
    [Route("materialReport")]
    public IActionResult Post([FromBody] MaterialReportEditDto dto)
    {
        return _mrService.Edit(dto, out var failReason) ? Success() : Error(failReason);
    }
}