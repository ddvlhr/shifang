using Core.Dtos;
using Core.Dtos.Specification;
using Infrastructure.Services.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Specification;

[Authorize]
public class MeasureTypeIndicatorsController : BaseController
{
    private readonly IMeasureTypeIndicatorsService _mtiService;

    public MeasureTypeIndicatorsController(IMeasureTypeIndicatorsService mtiService)
    {
        _mtiService = mtiService;
    }

    [HttpGet]
    [Route("measureTypeIndicators")]
    public IActionResult GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _mtiService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost]
    [Route("measureTypeIndicators")]
    public IActionResult Post([FromBody] MeasureTypeIndicatorsEditDto dto)
    {
        return _mtiService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("measureTypeIndicators/{id}")]
    public IActionResult GetInfo(int id)
    {
        var data = _mtiService.GetInfo(id);

        return Success(data);
    }
}