using System;
using Core.Dtos.Statistics;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Statistics;

[Authorize]
public class FilterMeasureQualityController : BaseController
{
    private readonly IFilterMeasureQualityService _fmqService;

    public FilterMeasureQualityController(IFilterMeasureQualityService fmqService)
    {
        _fmqService = fmqService;
    }

    [HttpGet]
    [Route("filterMeasureQuality")]
    public IActionResult Search([FromQuery] StatisticQueryInfoDto dto)
    {
        var result = _fmqService.Search(dto);
        return Success(result);
    }

    [HttpPost]
    [Route("filterMeasureQuality")]
    public IActionResult Download([FromBody] StatisticQueryInfoDto dto)
    {
        var file = _fmqService.Download(dto);
        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"滤棒检测合格率{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }
}