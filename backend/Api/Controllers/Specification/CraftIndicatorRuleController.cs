using Core.Dtos;
using Core.Dtos.Specification;
using Infrastructure.Services.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Specification;

[Authorize]
public class CraftIndicatorRuleController : BaseController
{
    private readonly ICraftIndicatorRuleService _cirService;

    public CraftIndicatorRuleController(ICraftIndicatorRuleService cirService)
    {
        _cirService = cirService;
    }

    [HttpGet]
    [Route("craftIndicatorRule")]
    public IActionResult GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _cirService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("craftIndicatorRule")]
    public IActionResult Post([FromBody] CraftIndicatorEditDto dto)
    {
        return _cirService.Edit(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("craftIndicatorRule/{id}")]
    public IActionResult Get(int id)
    {
        var data = _cirService.Get(id);
        return Success(data);
    }
}