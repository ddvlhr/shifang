using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;
/// <summary>
/// 卷接机管理
/// </summary>
[Authorize]
public class VolumePickUpController : BaseController
{
    private readonly IVolumePickUpService _vpService;

    public VolumePickUpController(IVolumePickUpService vpService)
    {
        _vpService = vpService;
    }

    [HttpGet, Route("volumePickUp")]
    public IActionResult GetVolumePickUps([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _vpService.GetVolumePickUps(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("volumePickUp")]
    public IActionResult Edit([FromBody] BaseEditDto dto)
    {
        return _vpService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    [HttpGet, Route("volumePickUp/options")]
    public IActionResult GetOptions()
    {
        return Success(_vpService.GetOptions());
    }
}