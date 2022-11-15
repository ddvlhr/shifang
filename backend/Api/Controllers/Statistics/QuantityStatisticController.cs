using System;
using Core.Dtos.QuantityStatistic;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Statistics;

[Authorize]
public class QuantityStatisticController : BaseController
{
    private readonly IQuantityStatisticService _qsService;

    public QuantityStatisticController(IQuantityStatisticService qsService)
    {
        _qsService = qsService;
    }

    [HttpGet]
    [Route("quantityStatistic")]
    public IActionResult GetInfo([FromQuery] QuantityStatisticQueryInfoDto dto)
    {
        return Success(_qsService.GetInfo(dto));
    }

    [HttpPost]
    [Route("quantityStatistic")]
    public IActionResult Download([FromBody] QuantityStatisticQueryInfoDto dto)
    {
        var file = _qsService.Download(dto);
        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"定量统计分析{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }
}