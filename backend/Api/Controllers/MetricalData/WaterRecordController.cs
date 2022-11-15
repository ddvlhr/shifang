using Core.Dtos.MetricalData;
using Infrastructure.Services.MetricalData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.MetricalData;

[Authorize]
public class WaterRecordController : BaseController
{
    private readonly IWaterRecordService _wrService;

    public WaterRecordController(IWaterRecordService wrService)
    {
        _wrService = wrService;
    }

    [HttpPost]
    [Route("waterRecord")]
    public IActionResult Add([FromBody] WaterRecordEditDto dto)
    {
        return _wrService.Add(dto, out var water, out var failReason) ? Success(water) : Error(failReason);
    }

    [HttpGet]
    [Route("waterRecord/{id}")]
    public IActionResult GetWaterRecordByGroupId(int id)
    {
        return Success(_wrService.GetWaterRecordByGroupId(id));
    }
}