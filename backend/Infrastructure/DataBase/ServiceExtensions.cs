using System;
using System.IO;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataBase;

public static class ServiceExtensions
{
    private const string ConnectionString = "mysql";

    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(c => { c.AddConsole(); });

    public static void AddDataServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true);
        var config = builder.Build();
        var settings = config.GetSection("Settings").Get<Settings>();
        var connectionString = NormalizeConnectionString(
            configuration.GetConnectionString(settings.MySqlServerName),
            out var createTemporary);
        if (createTemporary)
        {
            var logger = LoggerFactory.CreateLogger("StartUp");
            logger.LogCritical(
                $"没有配置数据库连接字符串。可以用 \"{connectionString}\" 来配置数据库连接字符串。");
            configuration[ConnectionString] = connectionString;
        }

        var useDatabase = PrepareDatabase(connectionString);
        services.AddDbContext<ApplicationDbContext>(options => { useDatabase(options); });
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static string NormalizeConnectionString(string connectionString,
        out bool createTemporary)
    {
        createTemporary = string.IsNullOrWhiteSpace(connectionString);
        return createTemporary
            ? "server=localhost;port=3306;SslMode=none;database=fuyang2020;uid=root;password=admin;Charset=utf8"
            : connectionString;
    }

    private static Action<DbContextOptionsBuilder> PrepareDatabase(
        string connectionString)
    {
        return options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
            x =>
            {
                x.MigrationsAssembly("Infrastructure");
                x.MigrationsHistoryTable("__efmigrationshistory");
            });
    }
}