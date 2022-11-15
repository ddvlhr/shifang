using System;
using Core.Dtos.WorkShopQuality;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Statistics;

[Authorize]
public class WorkShopQualityController : BaseController
{
    private readonly IWorkShopQualityService _wsqService;

    public WorkShopQualityController(IWorkShopQualityService wsqService)
    {
        _wsqService = wsqService;
    }

    [HttpGet]
    [Route("workShopQuality")]
    public IActionResult GetInfo([FromQuery] WorkShopQualityQueryInfoDto dto)
    {
        return Success(_wsqService.GetInfo(dto));
    }

    [HttpPost]
    [Route("workShopQuality/download")]
    public IActionResult Download([FromBody] WorkShopQualityQueryInfoDto dto)
    {
        var file = _wsqService.Download(dto);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"车间质量考核报表{DateTime.Now:yyyy-MM-dd_HH-mm-ss}");
    }
}