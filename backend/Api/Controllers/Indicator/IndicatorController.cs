using System.Collections.Generic;
using Core.Dtos.Indicator;
using Core.Entities;
using Infrastructure.Services.Indicator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Indicator;

[Authorize]
public class IndicatorController : BaseController
{
    private readonly IIndicatorService _iService;

    public IndicatorController(IIndicatorService iService)
    {
        _iService = iService;
    }

    [HttpGet]
    [Route("indicator")]
    public IActionResult Get([FromQuery] IndicatorQueryInfoDto dto)
    {
        var list = _iService.Get(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("indicator")]
    public IActionResult Post([FromBody] IndicatorEditDto dto)
    {
        return _iService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpPut]
    [Route("indicator")]
    public IActionResult Put([FromBody] IndicatorEditDto dto)
    {
        return _iService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("indicator/options")]
    public IActionResult GetOptions()
    {
        return Success(_iService.GetIndicatorOptions(IndicatorProject.None));
    }

    [HttpPost]
    [Route("indicator/rules")]
    public IActionResult GetRuleList(List<int> ids)
    {
        return Success(_iService.GetRuleList(ids));
    }

    [HttpGet]
    [Route("indicator/options/craft")]
    public IActionResult GetChemicalOptions()
    {
        return Success(_iService.GetIndicatorOptions(IndicatorProject.Craft));
    }
}