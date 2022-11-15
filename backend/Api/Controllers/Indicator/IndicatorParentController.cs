using Core.Dtos;
using Infrastructure.Services.Indicator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Indicator;

[Authorize]
public class IndicatorParentController : BaseController
{
    private readonly IIndicatorParentService _ipService;

    public IndicatorParentController(IIndicatorParentService ipService)
    {
        _ipService = ipService;
    }

    [HttpGet]
    [Route("indicatorParent")]
    public IActionResult Get(
        [FromQuery] BaseQueryInfoDto dto)
    {
        var result = _ipService.Get(dto, out var total);
        return Success(new { list = result, total });
    }

    [HttpPost]
    [Route("indicatorParent")]
    public IActionResult Post([FromBody] BaseEditDto dto)
    {
        return _ipService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpPut]
    [Route("indicatorParent")]
    public IActionResult Put([FromBody] BaseEditDto dto)
    {
        return _ipService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("indicatorParent/options")]
    public IActionResult Options()
    {
        return Success(_ipService.GetIndicatorParentOptions());
    }
}