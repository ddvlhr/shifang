using Core.Dtos;
using Infrastructure.Services.MetricalData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.MetricalData;
/// <summary>
/// 标定数据管理
/// </summary>
[Authorize]
public class CalibrationController : BaseController
{
    private readonly ICalibrationService _calibrationService;

    public CalibrationController(ICalibrationService calibrationService)
    {
        _calibrationService = calibrationService;
    }

    [HttpGet, Route("calibration")]
    public IActionResult GetCalibrations([FromQuery] BaseQueryInfoDto dto)
    {
        var list = _calibrationService.GetCalibrations(dto, out var total);
        return Success(new { list, total });
    }
}