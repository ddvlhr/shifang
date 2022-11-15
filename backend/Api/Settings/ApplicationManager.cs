using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Hosting;

namespace Api.Settings;

public class ApplicationManager
{
    private static ApplicationManager _appManager;
    private bool _running;
    private CancellationTokenSource _tokenSource;
    private IWebHost _web;

    public ApplicationManager()
    {
        _running = false;
        Restarting = false;
    }

    public bool Restarting { get; private set; }

    public static ApplicationManager Load()
    {
        return _appManager ??= new ApplicationManager();
    }

    public void Start()
    {
        if (_running)
            return;

        if (_tokenSource != null && _tokenSource.IsCancellationRequested)
            return;

        _tokenSource = new CancellationTokenSource();
        _tokenSource.Token.ThrowIfCancellationRequested();
        _running = true;

        _web = new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .ConfigureAppConfiguration((hostingContext, config) => { })
            .Build();

        _web.RunAsync(_tokenSource.Token);
    }

    public void Stop()
    {
        if (!_running)
            return;

        _tokenSource.Cancel();
        _running = false;
    }

    public void Restart()
    {
        Stop();

        Restarting = true;
        _tokenSource = null;
    }
}