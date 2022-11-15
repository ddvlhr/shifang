using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using Core.Models;

namespace Infrastructure.Helper;

public static class IpHelper
{
    public static Settings SystemSettings { get; set; } = new();

    public static string getLocalIp()
    {
        var ip = "";
        var ipList = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (var item in ipList)
            if (item.AddressFamily == AddressFamily.InterNetwork)
                ip = item.MapToIPv4().ToString();
        return ip;
    }

    public static string getRemoteIp()
    {
        var httpClient = new HttpClient();
        // var uri = new Uri(SystemSettings.GetRemoteIpUrl);
        Console.WriteLine(SystemSettings.GetRemoteIpUrl);
        return SystemSettings.GetRemoteIpUrl;
    }
}