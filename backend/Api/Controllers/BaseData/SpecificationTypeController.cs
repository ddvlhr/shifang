using Core.Dtos;
using Core.Dtos.Specification;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class SpecificationTypeController : BaseController
{
    private readonly ISpecificationTypeService _stService;

    public SpecificationTypeController(ISpecificationTypeService stService)
    {
        _stService = stService;
    }

    /// <summary>
    /// 获取牌号类型列表
    /// </summary>
    /// <param name="dto">筛选条件</param>
    /// <returns></returns>
    [HttpGet]
    [Route("specificationType")]
    public IActionResult Get([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _stService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    /// <summary>
    ///     添加牌号类型
    /// </summary>
    /// <param name="dto">牌号类型信息</param>
    /// <returns></returns>
    [HttpPost]
    [Route("specificationType")]
    public IActionResult Post([FromBody] SpecificationTypeEditDto dto)
    {
        return _stService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    /// <summary>
    ///     更新牌号类型
    /// </summary>
    /// <param name="dto">牌号类型信息</param>
    /// <returns></returns>
    [HttpPut]
    [Route("specificationType")]
    public IActionResult Put([FromBody] SpecificationTypeEditDto dto)
    {
        return _stService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("specificationType/options")]
    public IActionResult Get()
    {
        return Success(_stService.GetOptions());
    }

    [HttpGet]
    [Route("specificationType/info/{id}")]
    public IActionResult GetSpecificationTypeInfo(int id)
    {
        var data = _stService.GetSpecificationTypeInfo(id);
        return Success(data);
    }

    [HttpGet, Route("base/specification/type/options")]
    public IActionResult GetOptions()
    {
        return Success(_stService.GetOptions());
    }
}