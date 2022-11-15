using System;
using Core.Dtos.AppearanceStatistic;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Statistics;

[Authorize]
public class AppearanceStatisticController : BaseController
{
    private readonly IAppearanceStatisticService _asService;

    public AppearanceStatisticController(IAppearanceStatisticService asService)
    {
        _asService = asService;
    }

    [HttpGet]
    [Route("appearanceStatistic")]
    public IActionResult GetInfo([FromQuery] QueryInfoDto dto)
    {
        var data = _asService.GetInfo(dto);
        return Success(data);
    }

    [HttpGet]
    [Route("appearanceStatistic/download")]
    public IActionResult DownloadData([FromQuery] QueryInfoDto dto)
    {
        var file = _asService.DownloadData(dto);
        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"外观指标统计{DateTime.Now:yyyy-MM-dd_HH-mm-ss}");
    }
}