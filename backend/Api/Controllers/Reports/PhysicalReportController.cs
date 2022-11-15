using System;
using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Reports;

[Authorize]
public class PhysicalReportController : BaseController
{
    private readonly IPhysicalReportAppearanceService _praService;
    private readonly IPhysicalReportService _prService;

    public PhysicalReportController(IPhysicalReportService prService, IPhysicalReportAppearanceService praService)
    {
        _prService = prService;
        _praService = praService;
    }

    [HttpGet]
    [Route("physicalReport")]
    public IActionResult GetTable([FromQuery] QueryInfoDto dto)
    {
        var list = _prService.GetTable(dto, out var total);

        return Success(new { list, total });
    }

    [HttpPost]
    [Route("physicalReport")]
    public IActionResult UpdateAppearance([FromBody] EditReportAppearanceDto dto)
    {
        return _praService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("physicalReport/{id}")]
    public IActionResult GetAppearances(int id)
    {
        return Success(_praService.GetReportAppearances(id));
    }

    [HttpGet]
    [Route("physicalReport/{id}/statistic")]
    public IActionResult GetStatistic(int id)
    {
        var dto = _prService.GetStatisticInfo(id, out var failReason);
        if (string.IsNullOrEmpty(failReason))
            return Success(dto);
        return Error(failReason);
    }

    [HttpPost]
    [Route("physicalReport/download")]
    public IActionResult Download(int id)
    {
        var file = _prService.Download(id);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"物测检验报表{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }
}