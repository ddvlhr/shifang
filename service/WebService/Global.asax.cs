using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using RTLib;

namespace WebService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            if (initSignalR())
            {
                AppContext.IsConnected = true;
            }
            var signalRSendThread = new Thread(sendSignalR)
            {
                IsBackground = true
            };
            signalRSendThread.Start();
        }

        private bool initSignalR()
        {
            try
            {
                var signalRServerUrl = ConfigurationManager.AppSettings["signalRUrl"];
                var serviceUser = new Dictionary<string, object>()
                {
                    {"userId", 10086},
                    {"userName", "DataService"},
                    {"machine", 0}
                };

                AppContext.HubConnection = new HubConnectionBuilder()
                    .WithUrl(signalRServerUrl, options =>
                    {
                        options.Headers.Add("access_token", JsonConvert.SerializeObject(serviceUser));
                    })
                    .WithAutomaticReconnect()
                    .Build();
            
                AppContext.HubConnection.StartAsync().Wait();
                return true;
            }
            catch (Exception ex)
            {
                RunLog.save(ex.ToString(), false);
                return false;
            }
        }

        private void sendSignalR()
        {
            try
            {
                string signalRMessageDelay = ConfigurationManager.AppSettings["signalRMessageDelay"];
                if (!int.TryParse(signalRMessageDelay, out int delay))
                {
                    delay = 30;
                }
                while (AppContext.IsConnected)
                {
                    if (AppContext.SignalRQueue.TryDequeue(out SignalRRequest request))
                    {
                        AppContext.HubConnection.InvokeCoreAsync(request.MethodName, request.Data);
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(delay));
                }
            }
            catch (Exception ex)
            {
                RunLog.save(ex.ToString(), false);
            }
        } 

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}