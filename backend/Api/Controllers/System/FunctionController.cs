using Core.Dtos.Function;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.System;

[Authorize]
public class FunctionController : BaseController
{
    private readonly IFunctionService _functionService;

    public FunctionController(IFunctionService functionService)
    {
        _functionService = functionService;
    }

    [HttpPost]
    [Route("function")]
    public IActionResult Post([FromBody] EditFunctionDto dto)
    {
        return _functionService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("function/menu/{id}")]
    public IActionResult GetMenuFunctions(int id)
    {
        var functions = _functionService.GetMenuFunctions(id);
        return Success(functions);
    }

    [HttpGet]
    [Route("function/{id}")]
    public IActionResult GetFunction(int id)
    {
        var function = _functionService.GetFunction(id);

        return Success(function);
    }

    [HttpPut]
    [Route("function")]
    public IActionResult Put([FromBody] EditFunctionDto dto)
    {
        return _functionService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpDelete]
    [Route("function/{id}")]
    public IActionResult Delete(int id)
    {
        return _functionService.Delete(id, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpGet]
    [Route("function/menuFunctions")]
    public IActionResult GetMenuFunctions()
    {
        return Success(_functionService.GetMenuFunctions());
    }
}