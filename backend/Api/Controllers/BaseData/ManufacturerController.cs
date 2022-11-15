using System.Collections.Generic;
using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class ManufacturerController : BaseController
{
    private readonly IManufacturerService _mfService;

    public ManufacturerController(IManufacturerService mfService)
    {
        _mfService = mfService;
    }

    [HttpGet]
    [Route("manufacturer")]
    public IActionResult GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _mfService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("manufacturer")]
    public IActionResult Add([FromBody] BaseEditDto dto)
    {
        return _mfService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("manufacturer")]
    public IActionResult Update([FromBody] BaseEditDto dto)
    {
        return _mfService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpDelete]
    [Route("manufacturer")]
    public IActionResult Delete([FromBody] IEnumerable<int> ids)
    {
        return _mfService.Delete(ids) ? Success() : Error();
    }
}