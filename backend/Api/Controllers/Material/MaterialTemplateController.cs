using Core.Dtos.Material;
using Infrastructure.Services.Material;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Material;

[Authorize]
public class MaterialTemplateController : BaseController
{
    private readonly IMaterialTemplateService _mtService;

    public MaterialTemplateController(IMaterialTemplateService mtService)
    {
        _mtService = mtService;
    }

    [HttpGet]
    [Route("materialTemplate")]
    public IActionResult GetTable([FromQuery] MaterialTemplateQueryInfoDto dto)
    {
        var list = _mtService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpGet]
    [Route("materialTemplate/{id}")]
    public IActionResult Get(int id)
    {
        var data = _mtService.Get(id);
        return Success(data);
    }

    [HttpPost]
    [Route("materialTemplate")]
    public IActionResult Post([FromBody] MaterialTemplateEditDto dto)
    {
        return _mtService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }
}