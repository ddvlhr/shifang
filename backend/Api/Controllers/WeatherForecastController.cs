using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers;

/// <summary>
///     天气
/// </summary>
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : BaseController
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy",
        "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     获取天气信息
    /// </summary>
    /// <returns>随机天气数据</returns>
    [HttpGet]
    public IActionResult Get()
    {
        var rng = new Random();
        var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        return Success(data);
    }

    /// <summary>
    ///     测试错误接口
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("error")]
    public IActionResult Error()
    {
        throw new ArgumentNullException();
    }
}