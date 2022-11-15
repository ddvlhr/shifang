using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class FactorySiteController : BaseController
{
    private readonly IFactorySiteService _fsService;

    public FactorySiteController(IFactorySiteService fsService)
    {
        _fsService = fsService;
    }

    [HttpGet]
    [Route("factorySite")]
    public IActionResult GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _fsService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("factorySite")]
    public IActionResult Add([FromBody] BaseEditDto dto)
    {
        return _fsService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("factorySite")]
    public IActionResult Update([FromBody] BaseEditDto dto)
    {
        return _fsService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("factorySite/specificationId/{id}")]
    public IActionResult GetBySpecificationId(int id)
    {
        return Success(_fsService.GetFactorySiteBySpecificationId(id));
    }
}