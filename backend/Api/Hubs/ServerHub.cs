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
using System.Reflection.PortableExecutable;
using Org.BouncyCastle.Crypto.IO;

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
            Machine = tokenObj["machine"]?.ToString() == "" ? 0 : int.Parse(tokenObj["machine"].ToString()),
            Instance = ""
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
        _logger.LogInformation($"收到机台 {machine} 推送的数据, groupId: {groupId}, testTime: {testTime}");
        var clients = OnlineUsers.Where(c => c.Machine == machine).Select(c => c.ConnectionId).ToList();
        await Clients.Clients(clients).SendAsync("ReceiveMetricalPushData", result);
    }

    public async Task PushManualData(int groupId, string instance, string testTime)
    {
        // 获取仪器名称第一位字母, 如果为 J Y B 中的其中一个, 则推送手工车间数据到对应订阅用户
        var workShopName = instance.Substring(0, 1);
        _logger.LogInformation($"收到车间 {workShopName} 推送的数据, groupId: {groupId}, testTime: {testTime}");
        var clients = OnlineUsers.Where(c => c.Instance.Contains(workShopName)).Select(c => c.ConnectionId).ToList();
        await Clients.Clients(clients).SendAsync("ReceiveManualPushData", new {groupId, testTime});
    }

    public Task LoginPushData(string connectionId, int machineId)
    {
        var client = OnlineUsers.FirstOrDefault(c => c.ConnectionId == connectionId);
        if (client != null)
            client.Machine = machineId;
        _logger.LogInformation($"机台看板: 机台 {machineId} 已登录");
        return Task.CompletedTask;
    }

    public Task LoginManualPushData(string connectionId, string workShopName)
    {
        var client = OnlineUsers.FirstOrDefault(c => c.ConnectionId == connectionId);
        if (client != null)
            client.Instance = workShopName;
        _logger.LogInformation($"手工看板: 车间 {workShopName} 已登录");
        return Task.CompletedTask;
    }

    public Task LogoutPushData(string connectionId)
    {
        var client = OnlineUsers.FirstOrDefault(c => c.ConnectionId == connectionId);
        _logger.LogInformation($"机台看板: 机台 {client?.Machine} 已登出");
        if (client != null)
            client.Machine = 0;
        return Task.CompletedTask;
    }

    public Task LogoutManualPushData(string connectionId)
    {
        var client = OnlineUsers.FirstOrDefault(c => c.ConnectionId == connectionId);
        _logger.LogInformation($"手工看板: 车间 {client?.Instance} 已登出");
        if (client != null)
            client.Instance = "";
        return Task.CompletedTask;
    }
}