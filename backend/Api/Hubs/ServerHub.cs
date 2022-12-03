using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Dtos.User;
using Infrastructure.Helper;
using Infrastructure.Response;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Ubiety.Dns.Core;
using Response = Infrastructure.Response.Response;

namespace Api.Hubs;

public class ServerHub: Hub
{
    private List<SignalRUserDto> _onlineUsers = new List<SignalRUserDto>();
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
        var user = new SignalRUserDto()
        {
            ConnectionId = connId,
            UserId = _accessor.HttpContext.getUserId(),
            UserName = _accessor.HttpContext.getUserName()
        };
        
        _onlineUsers.Add(user);

        Clients.Client(connId).SendAsync("ConnectResponse",
            new Response(0, user.UserName + " 连接成功", user));
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}