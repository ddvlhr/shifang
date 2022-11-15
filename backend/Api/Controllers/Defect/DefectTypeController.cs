using Core.Dtos;
using Infrastructure.Services.Defect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Defect;

[Authorize]
public class DefectTypeController: BaseController
{
    private readonly IDefectTypeService _dtService;

    public DefectTypeController(IDefectTypeService dtService)
    {
        _dtService = dtService;
    }

    [HttpGet, Route("defect/type")]
    public IActionResult GetDefectTypes([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _dtService.GetDefectTypes(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("defect/type")]
    public IActionResult Edit([FromBody] BaseEditDto dto)
    {
        return _dtService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    [HttpGet, Route("defect/type/options")]
    public IActionResult GetOptions()
    {
        return Success(_dtService.GetOptions());
    }
}