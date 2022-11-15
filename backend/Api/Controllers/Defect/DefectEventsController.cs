using Core.Dtos;
using Infrastructure.Services.Defect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Defect;

[Authorize]
public class DefectEventsController: BaseController
{
    private readonly IDefectEventsService _deService;

    public DefectEventsController(IDefectEventsService deService)
    {
        _deService = deService;
    }

    [HttpGet, Route("defect/events")]
    public IActionResult GetDefectEvents([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _deService.GetDefectEvents(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("defect/events")]
    public IActionResult Edit([FromBody] BaseEditDto dto)
    {
        return _deService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    [HttpGet, Route("defect/events/options")]
    public IActionResult GetOptions()
    {
        return Success(_deService.GetOptions());
    }
}