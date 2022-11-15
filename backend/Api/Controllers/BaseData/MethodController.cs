using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class MethodController : BaseController
{
    private readonly IMethodService _mService;

    public MethodController(IMethodService mService)
    {
        _mService = mService;
    }

    [HttpGet]
    [Route("method")]
    public IActionResult GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _mService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("method")]
    public IActionResult Add([FromBody] BaseEditDto dto)
    {
        return _mService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("method")]
    public IActionResult Update([FromBody] BaseEditDto dto)
    {
        return _mService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpGet]
    [Route("method/specificationId/{id}")]
    public IActionResult GetMethodBySpecificationId(int id)
    {
        return Success(_mService.GetMethodNameBySpecificationId(id));
    }
}