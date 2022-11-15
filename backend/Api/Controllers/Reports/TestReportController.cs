using System;
using System.Collections.Generic;
using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Reports;

public class TestReportController : BaseController
{
    private readonly ITestReportService _trService;
    private readonly ITestReportAppearanceService _traService;

    public TestReportController(ITestReportService trService,
        ITestReportAppearanceService traService)
    {
        _trService = trService;
        _traService = traService;
    }
    
    [HttpGet]
    [Route("report/test")]
    public IActionResult GetTable([FromQuery] QueryInfoDto dto)
    {
        var list = _trService.GetTable(dto, out var total);

        return Success(new { list, total });
    }

    [HttpPost]
    [Route("report/test")]
    public IActionResult UpdateAppearance([FromBody] EditReportAppearanceDto dto)
    {
        return _traService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("report/test/{id}")]
    public IActionResult GetAppearances(int id)
    {
        return Success(_traService.GetReportAppearances(id));
    }

    [HttpGet]
    [Route("report/test/statistic/{id}")]
    public IActionResult GetStatistic(int id)
    {
        var dto = _trService.GetStatisticInfo(id, out var failReason);
        if (string.IsNullOrEmpty(failReason))
            return Success(dto);
        return Error(failReason);
    }

    [HttpPost]
    [Route("report/test/download")]
    public IActionResult Download([FromBody] List<int> ids)
    {
        var file = _trService.Download(ids);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"成品报表{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }
}