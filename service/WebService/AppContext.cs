using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR.Client;

namespace WebService;

public static class AppContext
{
    public static HubConnection HubConnection;
    public static bool IsConnected = false;
    public static ConcurrentQueue<SignalRRequest> SignalRQueue = new ConcurrentQueue<SignalRRequest>();
}
