using System;
using System.Collections.Generic;
using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Reports;

[Authorize]
public class ProductReportController : BaseController
{
    private readonly IProductReportAppearanceService _praService;
    private readonly IProductReportService _prService;

    public ProductReportController(IProductReportService prService, IProductReportAppearanceService praService)
    {
        _prService = prService;
        _praService = praService;
    }

    [HttpGet]
    [Route("productReport")]
    public IActionResult GetTable([FromQuery] QueryInfoDto dto)
    {
        var list = _prService.GetTable(dto, out var total);

        return Success(new { list, total });
    }

    [HttpPost]
    [Route("productReport")]
    public IActionResult UpdateAppearance([FromBody] EditReportAppearanceDto dto)
    {
        return _praService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("productReport/{id}")]
    public IActionResult GetAppearances(int id)
    {
        return Success(_praService.GetReportAppearances(id));
    }

    [HttpGet]
    [Route("productReport/{id}/statistic")]
    public IActionResult GetStatistic(int id)
    {
        var dto = _prService.GetStatisticInfo(id, out var failReason);
        if (string.IsNullOrEmpty(failReason))
            return Success(dto);
        return Error(failReason);
    }

    [HttpPost]
    [Route("productReport/download")]
    public IActionResult Download([FromBody] List<int> ids)
    {
        var file = _prService.Download(ids);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"成品报表{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }
}