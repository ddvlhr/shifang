using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Core.Dtos.Equipment;
using Core.Dtos.User;
using Core.Entities;
using Core.Enums;
using Core.SugarEntities;
using Infrastructure.Extensions;
using Infrastructure.Response;
using Infrastructure.Services.System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Api.Hubs;

public class ServerHub: Hub
{
    public static List<SignalRUserDto> OnlineUsers = new List<SignalRUserDto>();
    public static List<Equipment> Equipments = new List<Equipment>();
    private readonly IUserService _userService;
    private readonly ILogger<ServerHub> _logger;
    private readonly IHttpContextAccessor _accessor;
    private readonly IEquipmentService _equipmentService;

    public ServerHub(IUserService userService,
        ILogger<ServerHub> logger,
        IHttpContextAccessor accessor,
        IEquipmentService equipmentService)
    {
        _userService = userService;
        _logger = logger;
        _accessor = accessor;
        _equipmentService = equipmentService;
    }

    public override Task OnConnectedAsync()
    {
        var connId = Context.ConnectionId;
        
        var token = "";
        token = _accessor.HttpContext?.Request.Query.ContainsKey("access_token") == true ? _accessor.HttpContext?.Request.Query["access_token"] : _accessor.HttpContext?.Request.Headers["access_token"];
        var tokenObj = JsonSerializer.Deserialize<JsonObject>(token);
        if (tokenObj.ContainsKey("instance"))
        {
            var instance = tokenObj["instance"]?.ToString();
            Equipment equipment;
            if (Equipments.Any(c => c.Instance == instance))
            {
                equipment = Equipments.First(c => c.Instance == instance);
                equipment.ConnectionId = connId;
                equipment.Ip = tokenObj["ip"]?.ToString();
                equipment.Version = tokenObj["version"]?.ToString();
                equipment.LastConnectTime = DateTime.Now;
                equipment.Status = OnlineStatus.Online;
            }
            else
            {
                equipment = new Equipment()
                {
                    Name = tokenObj["name"]?.ToString(),
                    Description = tokenObj["description"]?.ToString(),
                    Instance = tokenObj["instance"]?.ToString(),
                    Ip = tokenObj["ip"]?.ToString(),
                    Version = tokenObj["version"]?.ToString(),
                    EquipmentType = (EquipmentType) Convert.ToInt32(tokenObj["equipmentType"]?.ToString()),
                    Status = OnlineStatus.Online,
                    LastConnectTime = DateTime.Now,
                    ConnectionId = connId
                };
                Equipments.Add(equipment);
            }
            
            _equipmentService.AddEquipment(equipment);
            var dto = new EquipmentDto()
            {
                Name = equipment.Name,
                Description = equipment.Description,
                Instance = equipment.Instance,
                Ip = equipment.Ip,
                Version = equipment.Version,
                EquipmentType = equipment.EquipmentType,
                OnlineStatus = equipment.Status,
                Status = equipment.Status.toDescription(),
                LastConnectTime = equipment.LastConnectTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EquipmentTypeDesc = equipment.EquipmentType.toDescription()
            };
            Clients.All.SendAsync("EquipmentMessage", new Response(0, "", dto));
        }
        var user = new SignalRUserDto()
        {
            ConnectionId = connId,
            UserId = tokenObj["userId"]?.ToString(),
            UserName = tokenObj["userName"]?.ToString(),
            Machine = tokenObj["machine"]?.ToString() == "" ? 0 : int.Parse(tokenObj["machine"].ToString())
        };
        
        OnlineUsers.Add(user);

        Clients.Client(connId).SendAsync("ConnectResponse",
            new Response(0, user.UserName + " 连接成功", user));
        _logger.LogWarning("SignalR已连接:{ConnId}", connId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var connId = Context.ConnectionId;
        var user = OnlineUsers.Find(c => c.ConnectionId == connId);
        var count = OnlineUsers.RemoveAll(c => c.ConnectionId == connId);
        if (Equipments.Any(c => c.ConnectionId == connId))
        {
            var equipment = Equipments.First(c => c.ConnectionId == connId);
            _equipmentService.Offline(equipment.Instance);
            var dto = new EquipmentDto()
            {
                Name = equipment.Name,
                Description = equipment.Description,
                Instance = equipment.Instance,
                Ip = equipment.Ip,
                Version = equipment.Version,
                EquipmentType = equipment.EquipmentType,
                OnlineStatus = OnlineStatus.Offline,
                Status = OnlineStatus.Offline.toDescription(),
                LastConnectTime = equipment.LastConnectTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EquipmentTypeDesc = equipment.EquipmentType.toDescription()
            };
            Equipments.Remove(equipment);
            Clients.All.SendAsync("EquipmentMessage", new Response(0, "", dto));
        }
        if (user != null)
        {
            Clients.Client(connId).SendAsync("DisconnectResponse", new Response(1000, "断开链接", true));
        }
        _logger.LogWarning("SignalR已断开连接: {" + connId);
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
        await Clients.Clients(OnlineUsers.Select(c => c.ConnectionId).ToList()).SendAsync("ReceiveMessage", result);
    }

    public async Task PushMetricalData(int groupId, int machine, string testTime)
    {
        var result = new Response(0, "", new {groupId, testTime});
        var clients = OnlineUsers.Where(c => c.Machine == machine).Select(c => c.ConnectionId).ToList();
        await Clients.Clients(clients).SendAsync("ReceiveMetricalPushData", result);
    }

    public Task LoginPushData(string connectionId, int machineId)
    {
        var client = OnlineUsers.FirstOrDefault(c => c.ConnectionId == connectionId);
        if (client != null)
            client.Machine = machineId;
        return Task.CompletedTask;
    }

    public Task LogoutPushData(string connectionId)
    {
        var client = OnlineUsers.FirstOrDefault(c => c.ConnectionId == connectionId);
        if (client != null)
            client.Machine = 0;
        return Task.CompletedTask;
    }
}