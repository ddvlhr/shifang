using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dtos.Report;
using Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reports;

/// <summary>
/// 工艺纪律执行情况
/// </summary>
[Authorize]
public class ProcessDisciplineReportController : BaseController
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
    public async Task<IActionResult> GetTable([FromQuery] ProcessDisciplineReportQueryInfoDto dto)
    {
        var result = await _pdrService.GetTableAsync(dto);
        return Success(result);
    }

    [HttpGet("report/processDiscipline/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _pdrService.GetByIdAsync(id);
        return result.Success ? Success(result.Data) : Error(result.Message);
    }

    /// <summary>
    /// 新增/编辑工艺纪律执行情况表
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost, Route("report/processDiscipline")]
    public async Task<IActionResult> Edit([FromBody] ProcessDisciplineReportInfoDto dto)
    {
        var result = await _pdrService.EditAsync(dto);
        return result.Success ? Success(msg: result.Message) : Error(result.Message);
    }

    /// <summary>
    /// 删除工艺纪律执行情况表
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost, Route("report/processDiscipline/remove")]
    public async Task<IActionResult> Remove(List<int> ids)
    {
        var result = await _pdrService.RemoveAsync(ids);
        return result.Success ? Success(msg: result.Message) : Error(result.Message);
    }
}