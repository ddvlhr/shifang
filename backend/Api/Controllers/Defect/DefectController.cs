using Core.Dtos.Defect;
using Infrastructure.Services.Defect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Defect;

/// <summary>
/// 缺陷管理
/// </summary>
[Authorize]
public class DefectController: BaseController
{
    private readonly IDefectService _dService;

    public DefectController(IDefectService dService)
    {
        _dService = dService;
    }

    [HttpGet, Route("defect")]
    public IActionResult GetDefects([FromQuery] DefectQueryInfoDto dto)
    {
        var list = _dService.GetDefects(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("defect")]
    public IActionResult EditDefect([FromBody] DefectInfoDto dto)
    {
        return _dService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    [HttpGet, Route("defect/options")]
    public IActionResult GetDefectOptions()
    {
        return Success(_dService.GetOptions());
    }
}