using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

/// <summary>
///     机台信息管理
/// </summary>
[Authorize]
public class MachineController : BaseController
{
    private readonly IMachineService _machineService;

    public MachineController(IMachineService machineService)
    {
        _machineService = machineService;
    }

    /// <summary>
    ///     获取机台列表
    /// </summary>
    /// <param name="dto">查询条件</param>
    /// <returns></returns>
    [HttpGet]
    [Route("machine")]
    public IActionResult Get([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _machineService.GetTable(dto, out var total);

        return Success(new { list, total });
    }

    /// <summary>
    ///     新增机台
    /// </summary>
    /// <param name="dto">机台信息</param>
    /// <returns></returns>
    [HttpPost]
    [Route("machine")]
    public IActionResult Post([FromBody] BaseEditDto dto)
    {
        return _machineService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    /// <summary>
    ///     更新机台
    /// </summary>
    /// <param name="dto">机台信息</param>
    /// <returns></returns>
    [HttpPut]
    [Route("machine")]
    public IActionResult Put([FromBody] BaseEditDto dto)
    {
        return _machineService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet, Route("machine/options")]
    public IActionResult GetOptions()
    {
        return Success(_machineService.GetOptions());
    }
}