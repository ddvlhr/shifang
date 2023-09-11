using System;
using System.IO;
using System.Reflection;
using System.Text;
using Api.BackgroundServices;
using Api.Hubs;
using Api.Settings;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

builder.Host.ConfigureLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Warning);
    logging.AddFile();
}).ConfigureServices(services =>
{
    services.AddHostedService<OnlineUserBackgroundService>();
    // services.AddHostedService<OnlineEquipmentsBackgroundService>();
    //services.AddHostedService<ServerInfoBackgroundService>();
}).ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.shifang.json", optional: true, reloadOnChange: true);
    // config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
    config.AddCommandLine(args);
});

services.AddControllers();
services.AddSignalR();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "数据采集与分析系统 API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "在下框中输入请求头中需要添加Jwt授权Token: Bearer Token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile =
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
});

services.AddCors(options =>
{
    options.AddPolicy("ShiFang", b =>
    {
        // var allowOrigins = configuration.GetSection("allowOrigins").Get<string[]>();
        b.SetIsOriginAllowed(_ => true)
        // b.WithOrigins(allowOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

services.Configure<Token>(configuration.GetSection("token"));
var token = configuration.GetSection("token").Get<Token>();

services.Configure<Core.Models.Settings>(configuration.GetSection("settings"));
// _appConfiguration.GetSection(nameof(Core.Models.Settings).ToUpperInvariant()).Bind(AppSettings.Settings);

services.AddAuthentication(c =>
{
    c.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;
    c.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(c =>
{
    c.RequireHttpsMetadata = false;
    c.SaveToken = true;
    c.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(token.Secret)),
        ValidIssuer = token.Issuer,
        ValidAudience = token.Audience,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

services.AddHttpContextAccessor();
services.AddLazyResolution();
services.ConfigureWriteAble<ShiFangSettings>(configuration.GetSection("ShiFangSettings"));
services.AddDataServices(configuration);
services.AddSqlSugarSetup(configuration);
services.AddAutoDi();
services.LoadServices();
// services.AddWatchDogServices(opt =>
// {
//     opt.IsAutoClear = true;
//     opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
// });

var app = builder.Build();

// 将跨域配置放在 UseHttpsRedirection 之前, 保证非 HTTPS 请求也能通过跨域

// app.UseHttpsRedirection();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "数据采集与分析系统 API v1");
    c.RoutePrefix = string.Empty;
});

// app.UseOperatingLogging();

// app.UseWatchDogExceptionLogger();

app.UseExceptionHandle();

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("ShiFang");

app.UseAuthentication();

app.UseAuthorization();

// app.UseWatchDog(opt =>
// {
//     opt.WatchPageUsername = "admin";
//     opt.WatchPagePassword = "123456";
// });

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    // SignalR 跨域配置，在IIS中部署时需要启用WebSocket
    endpoints.MapHub<ServerHub>("/ServerHub").RequireCors(t => t.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

app.Run();