using System;
using Core.Dtos.CraftAssessment;
using Infrastructure.Services.Statistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Api.Controllers.Statistics;

public class CraftAssessmentController : BaseController
{
    private readonly ICraftAssessmentService _caService;

    public CraftAssessmentController(ICraftAssessmentService caService)
    {
        _caService = caService;
    }

    [HttpGet, Route("statistic/craft/assessment")]
    public IActionResult Query([FromQuery] QueryInfoDto dto)
    {
        return Success(_caService.Search(dto));
    }

    [HttpGet, Route("statistic/craft/assessment/download")]
    public IActionResult Download([FromQuery] QueryInfoDto dto)
    {
        var file = _caService.Download(dto);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }
}