using System.Threading.Tasks;
using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class DisciplineClassController : BaseController
{
    private readonly IDisciplineClassService _disciplineClassService;
    public DisciplineClassController(IDisciplineClassService disciplineClassService)
    {
        _disciplineClassService = disciplineClassService;
    }

    [HttpGet("disciplineClass")]
    public async Task<IActionResult> GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var result = await _disciplineClassService.GetTableAsync(dto);
        return Success(result);
    }

    [HttpGet("disciplineClass/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _disciplineClassService.GetByIdAsync(id);
        return result.Success ? Success(result.Data) : Error(result.Message);
    }

    [HttpPost("disciplineClass")]
    public async Task<IActionResult> Edit([FromBody] BaseEditDto dto)
    {
        var result = await _disciplineClassService.EditAsync(dto);
        return result.Success ? Success(msg: result.Message) : Error(result.Message);
    }

    [HttpGet("disciplineClass/options")]
    public async Task<IActionResult> GetOptions()
    {
        var result = await _disciplineClassService.GetOptionsAsync();
        return Success(result);
    }
}