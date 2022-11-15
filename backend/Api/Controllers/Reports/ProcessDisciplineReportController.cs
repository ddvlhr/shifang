using System.Collections.Generic;
using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

/// <summary>
/// 工艺纪律执行情况
/// </summary>
[Authorize]
public class ProcessDisciplineReportController: BaseController
{
    private readonly IProcessDisciplineReportService _pdrService;

    public ProcessDisciplineReportController(IProcessDisciplineReportService pdrService)
    {
        _pdrService = pdrService;
    }

    /// <summary>
    /// 获取工艺纪律执行情况表
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpGet, Route("report/processDiscipline")]
    public IActionResult GetTable([FromQuery] ProcessDisciplineReportQueryInfoDto dto)
    {
        var list = _pdrService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    /// <summary>
    /// 新增/编辑工艺纪律执行情况表
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost, Route("report/processDiscipline")]
    public IActionResult Edit([FromBody] ProcessDisciplineReportInfoDto dto)
    {
        return _pdrService.Edit(dto, out var message) ? Success(msg: message) : Error(message);
    }

    /// <summary>
    /// 删除工艺纪律执行情况表
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost, Route("report/processDiscipline/remove")]
    public IActionResult Remove(List<int> ids)
    {
        return _pdrService.Remove(ids, out var message) ? Success(msg: message) : Error(message);
    }
}