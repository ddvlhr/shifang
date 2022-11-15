using System.Collections.Generic;
using Core.Dtos.WorkShopQuality;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class WorkShopQualityPointRuleController : BaseController
{
    private readonly IWorShopQualityPointRuleService _wsqprService;

    public WorkShopQualityPointRuleController(IWorShopQualityPointRuleService wsqprService)
    {
        _wsqprService = wsqprService;
    }

    [HttpGet]
    [Route("workShopQualityPointRule")]
    public IActionResult GetTable([FromQuery] WorkShopQualityPointRuleQueryInfoDto dto)
    {
        var list = _wsqprService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("workShopQualityPointRule")]
    public IActionResult Add([FromBody] WorkShopQualityPointRuleEditDto dto)
    {
        return _wsqprService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut]
    [Route("workShopQualityPointRule")]
    public IActionResult Edit([FromBody] WorkShopQualityPointRuleEditDto dto)
    {
        return _wsqprService.Edit(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpDelete]
    [Route("workShopQualityPointRule")]
    public IActionResult Delete(IEnumerable<int> ids)
    {
        return _wsqprService.Delete(ids) ? Success() : Error();
    }

    [HttpGet]
    [Route("workShopQualityPointRule/{id}")]
    public IActionResult GetInfo(int id)
    {
        return Success(_wsqprService.GetInfo(id));
    }
}