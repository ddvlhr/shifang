using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class MeasureTypeController : BaseController
{
    private readonly IMeasureTypeService _mtService;

    public MeasureTypeController(IMeasureTypeService mtService)
    {
        _mtService = mtService;
    }

    /// <summary>
    ///     获取测量类型列表
    /// </summary>
    /// <param name="dto">筛选条件</param>
    /// <returns></returns>
    [HttpGet]
    [Route("measureType")]
    public IActionResult Get([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _mtService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    /// <summary>
    ///     新增测量类型
    /// </summary>
    /// <param name="dto">测量类型信息</param>
    /// <returns></returns>
    [HttpPost]
    [Route("measureType")]
    public IActionResult Post([FromBody] BaseEditDto dto)
    {
        return _mtService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    /// <summary>
    ///     更新测量类型
    /// </summary>
    /// <param name="dto">测量类型信息</param>
    /// <returns></returns>
    [HttpPut]
    [Route("measureType")]
    public IActionResult Put([FromBody] BaseEditDto dto)
    {
        return _mtService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet, Route("base/measureType/options")]
    public IActionResult GetOptions()
    {
        return Success(_mtService.GetOptions());
    }
}