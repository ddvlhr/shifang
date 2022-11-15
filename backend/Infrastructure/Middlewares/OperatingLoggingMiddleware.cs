using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Middlewares;

public class OperatingLoggingMiddleware
{
    private readonly ILogger<OperatingLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly Stopwatch _stopwatch;

    public OperatingLoggingMiddleware(RequestDelegate next, ILogger<OperatingLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _stopwatch = new Stopwatch();
    }

    public async Task Invoke(HttpContext context)
    {
        _stopwatch.Start();

        var operating = new StringBuilder();
        var req = context.Request;
        var res = context.Response;
        if (!context.Response.HasStarted)
            // 设置header,允许前端获取文件名称
            res.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

        var hasDest = req.Headers.ContainsKey("sec-fetch-dest");
        if (!hasDest) await _next(context);

        var log = new OperatingLog { Path = req.Path };
        var dest = req.Headers["sec-fetch-dest"].ToString();
        var requestHeaders = JsonSerializer.Serialize(req.Headers.Keys);
        var requestHeaderValues = JsonSerializer.Serialize(req.Headers.Values);
        var method = req.Headers[":method"].ToString();
        operating.AppendLine($"request header keys: {requestHeaders}");
        operating.AppendLine($"request header values: {requestHeaderValues}");
        operating.AppendLine($"request method: {method}");
        operating.AppendLine($"request sec-fetch-dest: {dest}");
        operating.AppendLine($"request url: {req.Path}");
        Console.WriteLine($"request header: {requestHeaders}");
        Console.WriteLine($"request sec-fetch-dest: {dest}");
        Console.WriteLine($"request url: {req.Path}");
        if (req.Method.ToLower().Equals("post"))
        {
            req.EnableBuffering();

            using (var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
            {
                var value = await reader.ReadToEndAsync();
                log.Request = value.Length > 512 ? "请求表格数据" : value;
                operating.AppendLine($"request body: {value}");
                Console.WriteLine($"request body: {value}");
            }

            req.Body.Position = 0;
        }
        else if (req.Method.ToLower().Equals("get"))
        {
            log.Request = req.QueryString.Value.Length > 512 ? "请求数据过长" : req.QueryString.Value;
            operating.AppendLine($"request body: {req.QueryString.Value}");
            Console.WriteLine($"request body: {req.QueryString.Value}");
        }

        if (!context.Response.HasStarted)
        {
            var originalBodyStream = res.Body;

            await using var ms = new MemoryStream();
            res.Body = ms;
            await _next(context);

            res.Body.Position = 0;

            var resReader = new StreamReader(ms);
            var resContent = await resReader.ReadToEndAsync();
            log.Response = resContent.Length > 512 ? "" : resContent;
            operating.AppendLine($"response body: {resContent}");
            Console.WriteLine($"response body: {resContent}");

            res.Body.Position = 0;

            await ms.CopyToAsync(originalBodyStream);
            res.Body = originalBodyStream;
        }

        res.OnCompleted(() =>
        {
            _stopwatch.Stop();
            log.ExecutionTime = _stopwatch.ElapsedMilliseconds + " ms";
            // var user = context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // log.UserName = "";
            // _logRepo.Add(log);
            // _logRepo.Save();
            _logger.LogWarning(operating.ToString());
            return Task.CompletedTask;
        });
    }
}

public static class OperatingLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseOperatingLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OperatingLoggingMiddleware>();
    }
}