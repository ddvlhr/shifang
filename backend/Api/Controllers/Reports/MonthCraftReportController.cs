using System.Collections.Generic;
using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QueryInfoDto = Core.Dtos.MonthCraftReport.QueryInfoDto;

namespace Api.Controllers.Reports;

public class MonthCraftReportController : BaseController
{
    private readonly IMonthCraftReportService _mcrService;
    private readonly Core.Models.Settings _settings;

    public MonthCraftReportController(IMonthCraftReportService mcrService,
        IOptionsSnapshot<Core.Models.Settings> settings)
    {
        _mcrService = mcrService;
        _settings = settings.Value;
    }

    [HttpGet]
    [Route("monthCraftReport")]
    public IActionResult GetTable([FromQuery] QueryInfoDto dto)
    {
        var list = _mcrService.GetTable(dto, out var total);
        return Success(new { total, list });
    }

    [HttpPost]
    [Route("monthCraftReport")]
    public IActionResult Add([FromBody] MonthCraftReportEditDto dto)
    {
        return _mcrService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("monthCraftReport")]
    public IActionResult Update([FromBody] MonthCraftReportEditDto dto)
    {
        return _mcrService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("monthCraftReport/{id}")]
    public IActionResult GetInfo(int id)
    {
        var data = _mcrService.GetInfo(id, out var failReason);
        return string.IsNullOrEmpty(failReason) ? Success(data) : Error(failReason);
    }

    [HttpGet]
    [Route("monthCraftReport/items")]
    public IActionResult GetItems()
    {
        return Success(_settings.MonthCraftReportItems);
    }

    [HttpDelete]
    [Route("monthCraftReport")]
    public IActionResult Remote([FromBody] List<int> ids)
    {
        return _mcrService.Remove(ids, out var failReason) ? Success() : Error(failReason);
    }
}