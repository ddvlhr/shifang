using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

/// <summary>
/// 包装机管理
/// </summary>
[Authorize]
public class PackagingMachineController : BaseController
{
    private readonly IPackagingMachineService _pmService;

    public PackagingMachineController(IPackagingMachineService pmService)
    {
        _pmService = pmService;
    }

    [HttpGet, Route("packagingMachine")]
    public IActionResult GetPackagingMachines([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _pmService.GetPackagingMachines(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("packagingMachine")]
    public IActionResult Edit([FromBody] BaseEditDto dto)
    {
        return _pmService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    [HttpGet, Route("packagingMachine/options")]
    public IActionResult GetOptions()
    {
        return Success(_pmService.GetOptions());
    }
}