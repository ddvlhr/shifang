using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class TurnController : BaseController
{
    private readonly ITurnService _turnService;

    public TurnController(ITurnService turnService)
    {
        _turnService = turnService;
    }

    [HttpGet]
    [Route("turn")]
    public IActionResult Get([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _turnService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("turn")]
    public IActionResult Post([FromBody] BaseEditDto dto)
    {
        return _turnService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpPut]
    [Route("turn")]
    public IActionResult Put([FromBody] BaseEditDto dto)
    {
        return _turnService.Update(dto, out var failReason)
            ? Success()
            : Error(
                failReason);
    }

    [HttpGet, Route("base/turn/options")]
    public IActionResult Options()
    {
        return Success(_turnService.GetOptions());
    }
}