using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Threading.Tasks;
using Core.Dtos;
using Core.Dtos.MetricalData;
using Infrastructure.Services.MetricalData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Api.Controllers.MetricalData;

/// <summary>
///     测量数据管理
/// </summary>
[Authorize]
public class MetricalDataController : BaseController
{
    private readonly IMetricalDataService _mdService;
    private readonly Core.Models.Settings _settings;

    public MetricalDataController(IMetricalDataService mdService, IOptionsSnapshot<Core.Models.Settings> settings)
    {
        _mdService = mdService;
        _settings = settings.Value;
    }

    /// <summary>
    ///     获取测量数据组数据
    /// </summary>
    /// <param name="dto">查询条件</param>
    /// <returns></returns>
    [HttpGet]
    [Route("metricalData")]
    public IActionResult Get([FromQuery] MetricalDataQueryDto dto)
    {
        var list = _mdService.GetTable(dto, out var total);
        return Success(new { list, total });
    }

    /// <summary>
    ///     添加组数据
    /// </summary>
    /// <param name="dto">组信息</param>
    /// <returns></returns>
    [HttpPost]
    [Route("metricalData")]
    public IActionResult AddGroup([FromBody] MetricalDataGroupEditDto dto)
    {
        return _mdService.AddGroupInfo(dto, out var failReason) ? Success() : Error(failReason);
    }

    /// <summary>
    ///     更新组数据
    /// </summary>
    /// <param name="dto">组信息</param>
    /// <returns></returns>
    [HttpPut]
    [Route("metricalData")]
    public IActionResult UpdateGroup([FromBody] MetricalDataGroupEditDto dto)
    {
        return _mdService.UpdateGroupInfo(dto, out var groupId, out var failReason) ? Success() : Error(failReason);
    }

    /// <summary>
    ///     添加测量数据
    /// </summary>
    /// <param name="dto">测量数据</param>
    /// <returns></returns>
    [HttpPost]
    [Route("metricalData/data")]
    public IActionResult AddData([FromBody] MetricalDataEditDataDto dto)
    {
        return _mdService.AddDataInfo(dto, out var failReason) ? Success() : Error(failReason);
    }

    /// <summary>
    ///     更新测量数据
    /// </summary>
    /// <param name="dto">测量数据</param>
    /// <returns></returns>
    [HttpPut]
    [Route("metricalData/data")]
    public IActionResult UpdateData([FromBody] MetricalDataEditDataDto dto)
    {
        return _mdService.UpdateDataInfo(dto, out var failReason) ? Success() : Error(failReason);
    }

    /// <summary>
    ///     获取测量数据
    /// </summary>
    /// <param name="id">组数据ID</param>
    /// <returns></returns>
    [HttpGet]
    [Route("metricalData/data/{id}")]
    public IActionResult GetData(int id)
    {
        return Success(_mdService.GetDataInfo(id));
    }

    /// <summary>
    ///     删除数据
    /// </summary>
    /// <param name="ids">组数据ID集合</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("metricalData")]
    public IActionResult Delete([FromBody] List<int> ids)
    {
        return Success(_mdService.Delete(ids));
    }

    /// <summary>
    ///     获取统计信息
    /// </summary>
    /// <param name="id">组数据ID</param>
    /// <returns></returns>
    [HttpGet]
    [Route("metricalData/{id}")]
    public IActionResult GetStatistic(int id)
    {
        var result = _mdService.GetStatistic(id, out var failReason);
        return string.IsNullOrEmpty(failReason) ? Success(result) : Error(failReason);
    }

    [HttpGet]
    [Route("metricalData/specification/{id}")]
    public IActionResult GetDataInfo(int id)
    {
        string data;
        if (id == 0)
            data = _mdService.GetSpecifications();
        else
            data = _mdService.GetSpecification(id);
        return Success(data);
    }

    [HttpGet]
    [Route("metricalData/options")]
    public IActionResult GetOptions(int specificationId, string testDate, int type)
    {
        IEnumerable<BaseOptionDto> data;
        if (specificationId > 0 && !string.IsNullOrEmpty(testDate))
        {
            var date = Convert.ToDateTime(testDate);
            data = _mdService.GetOptions(specificationId, date, type);
        }
        else
        {
            data = _mdService.GetOptions();
        }

        return Success(data);
    }

    [HttpGet]
    [Route("metricalData/download/info")]
    public IActionResult DownloadInfo([FromQuery] MetricalDataQueryDto dto)
    {
        var file = _mdService.Download(dto);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"原始数据详细数据{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }

    [HttpGet]
    [Route("metricalData/download/statistic")]
    public IActionResult DownloadStatistic([FromQuery] MetricalDataQueryDto dto)
    {
        var file = _mdService.DownloadStatistic(dto);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"原始数据统计数据{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }
    
    [HttpGet]
    [Route("metricalData/download/statisticInfo")]
    public IActionResult DownloadStatisticInfo([FromQuery] MetricalDataQueryDto dto)
    {
        var file = _mdService.DownloadStatisticInfo(dto);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.Mappings[".xlsx"];
        return File(file, contentType, $"原始数据统计详细数据{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx");
    }

    [HttpGet]
    [Route("metricalData/specificationId/{specificationId}/measureTypeId/{measureTypeId}")]
    public IActionResult GetMetricalDataBySpecificationIdAndMeasureTypeId(int specificationId, int measureTypeId)
    {
        return Success(_mdService.GetMeasureDataBySpecificationIdAndMeasureTypeId(specificationId, measureTypeId));
    }

    [HttpGet("metricalData/statistic/{id}")]
    public IActionResult GetStatisticInfo(int id)
    {
        return Success(_mdService.GetStatisticInfo(id, out var message));
    }

    [HttpGet("metricalData/info/{id}")]
    public async Task<IActionResult> GetInfo(int id)
    {
        var list = await _mdService.GetMetricalDataInfoAsync(id);
        return Success(list);
    }

    [HttpGet("metricalData/info/manual/{id}/{workShopName}")]
    public async Task<IActionResult> GetManualInfo(int id, string workShopName)
    {
        var list = await _mdService.GetManualMetricalDataInfoAsync(id, workShopName);
        return Success(list);
    }

    [HttpGet("metricalData/info/manual/{workShopName}")]
    public IActionResult GetManualCheckerInfos(string workShopName)
    {
        var result = _mdService.GetManualCheckerInfos(workShopName);
        return Success(result);
    }
    /// <summary>
    /// 获取手工车间
    /// </summary>
    /// <returns></returns>

    [HttpGet("metricalData/getHandicraftWorkshop")]
    public async Task<IActionResult> GetHandicraftWorkshop()
    {
        var list = await _mdService.GetHandicraftWorkshopAsync();
        return Success(list);
    }

    /// <summary>
    /// 获取手工车间的测量数据
    /// </summary>
    /// <param name="WorkShopLetter">手工车间字符</param>
    /// <param name="PageSize"></param>
    /// <param name="PageNum"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("metricalData/GetHandicraftWorkshopMatrialData")]
    public  IActionResult getHandicraftWorkshopMatrialData(string WorkShopLetter, int PageSize, int PageNum)
    {
        var list =  _mdService.GetHandicraftWorkshopMatrialData( WorkShopLetter,  PageSize,  PageNum, out int total);
        return Success(list);
    }
    /// <summary>
    /// 获取手工车间统计数据
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("metricalData/GetHandicraftWorkshopStatisticsData")]
    public IActionResult GetHandicraftWorkshopStatisticsData(string WorkShopLetter, int PageSize, int PageNum)
    {
        var list = _mdService.GetHandicraftWorkshopMatrialData(WorkShopLetter, PageSize, PageNum, out int total);
        return Success(list);
    }
}