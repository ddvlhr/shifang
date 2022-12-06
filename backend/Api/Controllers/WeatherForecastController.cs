using System;
using System.Threading.Tasks;
using Api.Hubs;
using Core.Dtos.User;
using Infrastructure.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
    private readonly IHttpContextAccessor _accessor;
    private readonly IHubContext<ServerHub> _hubContext;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IHttpContextAccessor accessor,
        IHubContext<ServerHub> hubContext)
    {
        _logger = logger;
        _accessor = accessor;
        _hubContext = hubContext;
    }
    

    [HttpGet("signalR")]
    public async Task SendSignalRMessage(string message = "服务器推送测试消息", string userId = "121", string userName = "刘浩然")
    {
        var user = new SignalRUserDto()
        {
            UserId = userId,
            UserName = userName
        };

        var result = new Response(0, message, user);
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", JsonConvert.SerializeObject(result));
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