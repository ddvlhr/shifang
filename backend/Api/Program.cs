using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        // var appManager = ApplicationManager.Load();
        // do
        // {
        //     appManager.Start();
        // } while (appManager.Restarting);
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Warning);
                logging.AddFile();
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
    // .ConfigureServices(services =>
    // {
    //     services.AddHostedService<PhysicalDataConvert>();
    // });
}