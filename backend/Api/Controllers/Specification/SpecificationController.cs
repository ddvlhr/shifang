using Core.Dtos.Specification;
using Infrastructure.Services.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Specification;

/// <summary>
///     牌号管理
/// </summary>
[Authorize]
public class SpecificationController : BaseController
{
    private readonly ISpecificationService _spService;

    public SpecificationController(ISpecificationService spService)
    {
        _spService = spService;
    }

    /// <summary>
    ///     获取牌号列表
    /// </summary>
    /// <param name="dto">查询条件</param>
    /// <returns></returns>
    [HttpGet]
    [Route("specification")]
    public IActionResult Get([FromQuery] SpecificationQueryInfoDto dto)
    {
        var list = _spService.Get(dto, out var total);
        return Success(new { list, total });
    }

    /// <summary>
    ///     添加牌号
    /// </summary>
    /// <param name="dto">牌号信息和牌号规则信息</param>
    /// <returns></returns>
    [HttpPost]
    [Route("specification")]
    public IActionResult Post([FromBody] SpecificationEditDto dto)
    {
        return _spService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    /// <summary>
    ///     更新牌号信息
    /// </summary>
    /// <param name="dto">牌号信息和牌号规则</param>
    /// <returns></returns>
    [HttpPut]
    [Route("specification")]
    public IActionResult Put([FromBody] SpecificationEditDto dto)
    {
        return _spService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }

    /// <summary>
    ///     获取牌号信息
    /// </summary>
    /// <param name="id">牌号ID</param>
    /// <returns></returns>
    [HttpGet]
    [Route("specification/{id}")]
    public IActionResult GetInfo(int id)
    {
        return Success(_spService.GetInfo(id));
    }

    [HttpGet]
    [Route("specification/type/{id}")]
    public IActionResult GetSpecificationsByTypeId(int id)
    {
        return Success(_spService.GetSpecificationsByTypeId(id));
    }

    [HttpGet, Route("specification/options")]
    public IActionResult GetOptions()
    {
        return Success(_spService.GetOptions());
    }
}