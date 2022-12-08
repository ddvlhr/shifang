using System.Threading.Tasks;
using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class DisciplineClauseController : BaseController
{
    private readonly IDisciplineClauseService _disciplineClauseService;
    public DisciplineClauseController(IDisciplineClauseService disciplineClauseService)
    {
        _disciplineClauseService = disciplineClauseService;
    }

    [HttpGet("disciplineClause")]
    public async Task<IActionResult> GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var result = await _disciplineClauseService.GetTableAsync(dto);
        return Success(result);
    }

    [HttpGet("disciplineClause/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _disciplineClauseService.GetByIdAsync(id);
        return result.Success ? Success(result.Data) : Error(result.Message);
    }

    [HttpPost("disciplineClause")]
    public async Task<IActionResult> Edit([FromBody] BaseEditDto dto)
    {
        var result = await _disciplineClauseService.EditAsync(dto);
        return result.Success ? Success(result.Data) : Error(result.Message);
    }

    [HttpGet("disciplineClause/options")]
    public async Task<IActionResult> GetOptions()
    {
        var result = await _disciplineClauseService.GetOptionsAsync();
        return Success(result);
    }
}