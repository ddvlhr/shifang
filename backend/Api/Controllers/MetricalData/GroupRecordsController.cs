using Core.Dtos.GroupRecord;
using Infrastructure.Services.MetricalData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.MetricalData;

[Authorize]
[Route("api/groupRecords")]
public class GroupRecordsController : BaseController
{
    private readonly IGroupRecordService _grService;

    public GroupRecordsController(IGroupRecordService grService)
    {
        _grService = grService;
    }

    [HttpGet]
    public IActionResult GetInfo([FromQuery] GroupRecordQueryInfoDto dto)
    {
        var result = _grService.GetGroupRecords(dto, out var total);
        return Success(new { total, result });
    }
}