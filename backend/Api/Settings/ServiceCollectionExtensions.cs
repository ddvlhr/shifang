using Api.Settings.Impl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Api.Settings;

public static class ServiceCollectionExtensions
{
    public static void ConfigureWriteAble<T>(this IServiceCollection services,
        IConfigurationSection section,
        string file = "appsettings.json") where T : class, new()
    {
        services.Configure<T>(section);
        services.AddTransient<IWriteAbleOptions<T>>(provider =>
        {
            var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
            var env = provider.GetService<IWebHostEnvironment>();
            var options = provider.GetService<IOptionsMonitor<T>>();
            return new WriteAbleOptions<T>(env, options, configuration, section.Key, file);
        });
    }
}