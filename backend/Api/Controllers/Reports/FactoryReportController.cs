using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

[Authorize]
public class FactoryReportController : BaseController
{
    private readonly IFactoryReportService _frService;

    public FactoryReportController(IFactoryReportService frService)
    {
        _frService = frService;
    }

    [HttpGet]
    [Route("factoryReport")]
    public IActionResult GetTable([FromQuery] QueryInfoDto dto)
    {
        var list = _frService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpGet]
    [Route("factoryReport/{id}")]
    public IActionResult GetInfo(int id)
    {
        return Success(_frService.GetInfo(id));
    }

    [HttpPost]
    [Route("factoryReport")]
    public IActionResult Edit([FromBody] FactoryReportEditDto dto)
    {
        return _frService.Edit(dto) ? Success() : Error();
    }

    [HttpGet]
    [Route("factoryReport/download/{id}")]
    public IActionResult Download(int id)
    {
        return Success(_frService.GetReportInfo(id));
    }
}