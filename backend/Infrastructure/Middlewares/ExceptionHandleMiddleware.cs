using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Middlewares;

public class ExceptionHandleMiddleware
{
    private readonly ILogger<ExceptionHandleMiddleware> _logger;
    private readonly RequestDelegate _next;
    private Settings _settings;

    public ExceptionHandleMiddleware(RequestDelegate next,
        ILogger<ExceptionHandleMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IOptionsSnapshot<Settings> settings)
    {
        try
        {
            _settings = settings.Value;
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var code = StatusCodes.Status500InternalServerError;
        var msg = "服务器内部错误, 无法完成请求";
        var message = "";
        if (ex != null)
        {
            code = 1;
            msg = ex.Message == "" ? "未登录" : ex.Message;
            message = ex.ToString();
        }
        else
        {
            switch (context.Response.StatusCode)
            {
                case 401:
                    code = 401;
                    msg = "没有权限";
                    break;
                case 404:
                    code = 404;
                    msg = "未找到服务";
                    break;
                case 403:
                    msg = "服务器拒绝执行请求";
                    break;
                case 500:
                    msg = "服务器内部错误, 无法请求完成";
                    break;
                case 502:
                    msg = "请求错误";
                    break;
            }
        }

        if (message != "") _logger.LogError(message);
        //
        // var markdownHelper = new MarkdownHelper();
        // markdownHelper.AddTitle(1, "阜阳数采系统");
        // var markdownMsg = markdownHelper.MarkdownBuilder.ToString();
        var msgList = new List<string>
        {
            "StatusCode cannot be set because the response has already started.",
            "Response already started"
        };
        if (_settings.EnableErrorPush)
            if (!msgList.Contains(msg))
                DingTalkHelper.SendText(message, _settings.ErrorPushAt, false);
        // DingTalkHelper.SendMarkdown(msg, markdownMsg, phoneNumbers, false);
        var response = new Response.Response(code, msg, null);
        var result = JsonConvert.SerializeObject(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;
        return context.Response.WriteAsync(result);
    }
}

public static class ExceptionHandleMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandle(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandleMiddleware>();
    }
}