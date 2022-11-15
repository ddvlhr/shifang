using Core.Dtos;
using Infrastructure.Services.BaseData;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.BaseData;

public class ReportOrderController : BaseController
{
    private readonly IReportOrderService _roService;

    public ReportOrderController(IReportOrderService roService)
    {
        _roService = roService;
    }

    [HttpGet("reportOrder")]
    public IActionResult GetTable([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _roService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    [HttpPost("reportOrder")]
    public IActionResult Add([FromBody] BaseEditDto dto)
    {
        return _roService.Add(dto, out var failReason) ? Success() : Error(failReason);
    }

    [HttpPut("reportOrder")]
    public IActionResult Update([FromBody] BaseEditDto dto)
    {
        return _roService.Update(dto, out var failReason) ? Success() : Error(failReason);
    }
}