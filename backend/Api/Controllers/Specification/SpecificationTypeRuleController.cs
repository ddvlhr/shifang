using Core.Dtos;
using Core.Dtos.Specification;
using Infrastructure.Services.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Specification;

[Authorize]
public class SpecificationTypeRuleController : BaseController
{
    private readonly ISpecificationTypeRuleService _strService;

    public SpecificationTypeRuleController(ISpecificationTypeRuleService strService)
    {
        _strService = strService;
    }

    [HttpGet]
    [Route("specificationTypeRule")]
    public IActionResult GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _strService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpGet]
    [Route("specificationTypeRule/{id}")]
    public IActionResult Get(int id)
    {
        var data = _strService.Get(id);
        return Success(data);
    }

    [HttpPost]
    [Route("specificationTypeRule")]
    public IActionResult Post([FromBody] SpecificationTypeRuleEditDto dto)
    {
        return _strService.Edit(dto, out var failReason) ? Success() : Error(failReason);
    }
}