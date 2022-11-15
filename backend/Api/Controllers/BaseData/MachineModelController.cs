using Core.Dtos.MachineModel;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

[Authorize]
public class MachineModelController : BaseController
{
    private readonly IMachineModelService _mmService;

    public MachineModelController(IMachineModelService mmService)
    {
        _mmService = mmService;
    }

    [HttpGet]
    [Route("machineModel")]
    public IActionResult Get([FromQuery] MachineModelQueryInfoDto dto)
    {
        var list = _mmService.GetTable(dto, out var total);

        return Success(new { list, total });
    }

    [HttpPost]
    [Route("machineModel")]
    public IActionResult Post([FromBody] MachineModelEditDto dto)
    {
        return _mmService.Add(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }

    [HttpPut]
    [Route("machineModel")]
    public IActionResult Put([FromBody] MachineModelEditDto dto)
    {
        return _mmService.Update(dto, out var failReason)
            ? Success()
            : Error(failReason);
    }
}