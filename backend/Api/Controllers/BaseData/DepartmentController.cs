using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

/// <summary>
/// 部门管理
/// </summary>
[Authorize]
public class DepartmentController: BaseController
{
    private readonly IDepartmentService _dService;

    public DepartmentController(IDepartmentService dService)
    {
        _dService = dService;
    }

    /// <summary>
    /// 获取部门列表信息
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpGet, Route("department")]
    public IActionResult GetDepartments([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _dService.GetDepartments(dto, out var total);
        return Success(new { list, total });
    }

    /// <summary>
    /// 新增/修改部门信息
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost, Route("department")]
    public IActionResult Edit([FromBody] BaseEditDto dto)
    {
        return _dService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    /// <summary>
    /// 获取部门选项信息
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("department/options")]
    public IActionResult GetOptions()
    {
        return Success(_dService.GetOptions());
    }
}