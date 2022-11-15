using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

/// <summary>
/// 班组管理
/// </summary>
[Authorize]
public class TeamController : BaseController
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }
    [HttpGet, Route("team")]
    public IActionResult GetTeams([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _teamService.GetTeams(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost, Route("team")]
    public IActionResult Edit([FromBody] BaseEditDto dto)
    {
        return _teamService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    [HttpGet, Route("team/options")]
    public IActionResult GetOptions()
    {
        return Success(_teamService.GetOptions());
    }
}