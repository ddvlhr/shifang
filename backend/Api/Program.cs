﻿using System;
using System.IO;
using System.Reflection;
using System.Text;
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
using WatchDog;
using WatchDog.src.Enums;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Warning);
    logging.AddFile();
});

var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllers();
services.AddSignalR();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo {Title = "数据采集与分析系统 API", Version = "v1"});

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
            new string[] { }
        }
    });

    var xmlFile =
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, true);
});

services.AddCors(options =>
{
    options.AddPolicy("FuYang", b =>
    {
        var allowOrigins = configuration.GetSection("allowOrigins").Get<string[]>();
        b.WithOrigins(allowOrigins)
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
services.ConfigureWriteAble<Core.Models.Settings>(configuration.GetSection("Settings"));
services.AddDataServices(configuration);
services.AddSqlSugarSetup(configuration);
services.AddAutoDi();
services.LoadServices();
services.AddWatchDogServices(opt =>
{
    opt.IsAutoClear = true;
    opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "数据采集与分析系统 API v1");
    c.RoutePrefix = string.Empty;
});

// app.UseOperatingLogging();

app.UseWatchDogExceptionLogger();

app.UseExceptionHandle();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("FuYang");

app.UseAuthentication();

app.UseAuthorization();
        
app.UseWatchDog(opt =>
{
    opt.WatchPageUsername = "admin";
    opt.WatchPagePassword = "123456";
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ServerHub>("/ServerHub");
});

app.Run();