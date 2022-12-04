using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Dtos.User;
using Infrastructure.Helper;
using Infrastructure.Response;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
// using Response = Infrastructure.Response.Response;

namespace Api.Hubs;

public class ServerHub: Hub
{
    public static List<SignalRUserDto> OnlineUsers { get; set; } = new List<SignalRUserDto>();
    private readonly IUserService _userService;
    private readonly ILogger<ServerHub> _logger;
    private readonly IHttpContextAccessor _accessor;

    public ServerHub(IUserService userService,
        ILogger<ServerHub> logger,
        IHttpContextAccessor accessor)
    {
        _userService = userService;
        _logger = logger;
        _accessor = accessor;
    }

    public override Task OnConnectedAsync()
    {
        var connId = Context.ConnectionId;
        
        _logger.LogWarning("SignalR已连接");
        var token = _accessor.HttpContext?.Request.Query["access_token"];
        var userInfo = JsonSerializer.Deserialize<SignalRUserDto>(token);
        var user = new SignalRUserDto()
        {
            ConnectionId = connId,
            UserId = userInfo.UserId,
            UserName = userInfo.UserName
        };
        
        OnlineUsers.Add(user);

        Clients.Client(connId).SendAsync("ConnectResponse",
            new Response(0, user.UserName + " 连接成功", user));
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var connId = Context.ConnectionId;
        var user = OnlineUsers.Find(c => c.ConnectionId == connId);
        var count = OnlineUsers.RemoveAll(c => c.ConnectionId == connId);
        if (user != null)
        {
            Clients.Client(connId).SendAsync("DisconnectResponse", new Response(1000, "断开链接", true));
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string user, string msg)
    {
        var model = new SignalRUserDto()
        {
            ConnectionId = Context.ConnectionId,
            UserName = user
        };
        var result = new Response(0, msg, model);
        await Clients.Clients(OnlineUsers.Select(c => c.ConnectionId).ToList()).SendAsync("SendMessage", result);
    }
}