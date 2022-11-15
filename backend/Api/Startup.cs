using System;
using System.IO;
using System.Reflection;
using System.Text;
using Api.Settings;
using Core.Models;
using Infrastructure.DataBase;
using Infrastructure.Extensions;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api;

public class Startup
{
    private readonly IConfiguration _appConfiguration;

    public Startup(IConfiguration configuration)
    {
        _appConfiguration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

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
            options.AddPolicy("FuYang", builder =>
            {
                var allowOrigins = _appConfiguration.GetSection("allowOrigins").Get<string[]>();
                builder.WithOrigins(allowOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.Configure<Token>(_appConfiguration.GetSection("token"));
        var token = _appConfiguration.GetSection("token").Get<Token>();

        services.Configure<Core.Models.Settings>(_appConfiguration.GetSection("settings"));
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
        services.ConfigureWriteAble<Core.Models.Settings>(_appConfiguration.GetSection("Settings"));
        services.AddDataServices(_appConfiguration);
        services.AddAutoDi();
        services.LoadServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        app.UseHttpsRedirection();

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "数据采集与分析系统 API v1");
            c.RoutePrefix = string.Empty;
        });

        app.UseOperatingLogging();

        app.UseExceptionHandle();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("FuYang");

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}