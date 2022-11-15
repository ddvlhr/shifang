using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class WorkShopController : BaseController
{
    private readonly IWorkShopService _wsService;

    public WorkShopController(IWorkShopService wsService)
    {
        _wsService = wsService;
    }

    [HttpGet]
    [Route("workShop")]
    public IActionResult Get([FromQuery] BaseQueryInfoDto dto)
    {
        var result = _wsService.GetTable(dto, out var total);
        return Success(new { list = result, total });
    }

    [HttpPost]
    [Route("workShop")]
    public IActionResult Post([FromBody] BaseEditDto dto)
    {
        return _wsService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpPut]
    [Route("workShop")]
    public IActionResult Put([FromBody] BaseEditDto dto)
    {
        return _wsService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet, Route("base/workshop/options")]
    public IActionResult GetOptions()
    {
        var intOptions = _wsService.GetOptions(false);
        var stringOptions = _wsService.GetOptions(true);
        return Success(new { intOptions, stringOptions });
    }
}