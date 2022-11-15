using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class ModelController : BaseController
{
    private readonly IModelService _mService;

    public ModelController(IModelService mService)
    {
        _mService = mService;
    }

    [HttpGet]
    [Route("model")]
    public IActionResult GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _mService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("model")]
    public IActionResult Add([FromBody] BaseEditDto dto)
    {
        return _mService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("model")]
    public IActionResult Edit([FromBody] BaseEditDto dto)
    {
        return _mService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }
}