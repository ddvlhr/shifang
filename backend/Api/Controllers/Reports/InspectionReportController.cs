using System;
using System.Collections.Generic;
using Core.Dtos.MetricalData;
using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Reports;

[Authorize]
public class InspectionReportController : BaseController
{
    private readonly IInspectionReportAppearanceService _iraService;
    private readonly IInspectionReportService _irService;

    public InspectionReportController(IInspectionReportService irService, IInspectionReportAppearanceService iraService)
    {
        _irService = irService;
        _iraService = iraService;
    }

    [HttpGet]
    [Route("inspectionReport")]
    public IActionResult GetTable([FromQuery] QueryInfoDto dto)
    {
        var list = _irService.GetTable(dto, out var total);

        return Success(new { list, total });
    }

    [HttpPost]
    [Route("inspectionReport")]
    public IActionResult UpdateAppearance([FromBody] EditReportAppearanceDto dto)
    {
        return _iraService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("inspectionReport/{id}")]
    public IActionResult GetAppearances(int id)
    {
        return Success(_iraService.GetReportAppearances(id));
    }

    [HttpGet]
    [Route("inspectionReport/{id}/statistic")]
    public IActionResult GetStatistic(int id)
    {
        var dto = _irService.GetStatisticInfo(id, out var failReason);
        if (string.IsNullOrEmpty(failReason))
            return Success(dto);
        return Error(failReason);
    }

    [HttpPost]
    [Route("inspectionReport/download")]
    public IActionResult Download([FromBody] List<int> ids)
    {
        var file = _irService.Download(ids);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"巡检报表{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }

    [HttpPost]
    [Route("inspectionReport/add")]
    public IActionResult Add(MetricalDataGroupEditDto dto)
    {
        return _irService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }
}