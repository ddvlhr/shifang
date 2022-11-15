using System;
using System.IO;
using System.Net;
using System.Text;
using Core.Dtos.DingTalkHelperDtos;
using Newtonsoft.Json;

namespace Infrastructure.Helper;

public class DingTalkHelper
{
    public const string Keyword = "【fuyang2021】";

    public static string WebHookUrl =
        "https://oapi.dingtalk.com/robot/send?access_token=4738f30452afd400962d12ab3a84ac19ec631e39c3718f6d1380b869dee9dc84";

    public static readonly StringBuilder JsonStringBuilder = new();

    public static string Post(string data)
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(WebHookUrl);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = WebRequestMethods.Http.Post;
        if (data != null)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentLength = dataBytes.Length;
            using var reqStream = httpWebRequest.GetRequestStream();
            reqStream.Write(dataBytes, 0, dataBytes.Length);
            httpWebRequest.GetRequestStream().Write(dataBytes, 0, dataBytes.Length);
        }

        using var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        var responseStream = httpWebResponse.GetResponseStream();
        return new StreamReader(responseStream, Encoding.UTF8).ReadToEnd();
    }

    public static void SendText(string content, string[] phoneNumbers, bool isAtAll)
    {
        var normalMsg = new NormalMsg();
        var ip = $"ip地址: {IpHelper.getLocalIp()} \r";
        normalMsg.Text.Content = Keyword + ip + content;
        normalMsg.At.AtMobiles = phoneNumbers;
        normalMsg.At.IsAtAll = isAtAll;
        var json = JsonConvert.SerializeObject(normalMsg);
        JsonStringBuilder.Clear();
        JsonStringBuilder.Append(json);
        var msg = JsonStringBuilder.ToString();
        var returnJson = Post(msg);
        Console.Write(returnJson);
    }

    /// <summary>
    ///     发送 Link 类型
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="text">内容</param>
    /// <param name="imageUrl">图片链接</param>
    /// <param name="contentUrl">消息跳转链接</param>
    public static void SendLink(string title, string text, string imageUrl, string contentUrl)
    {
        JsonStringBuilder.Clear();
        JsonStringBuilder.Append("{\"msgtype\":\"link\",\"link\":{" +
                                 $"\"title\":\"{Keyword + title}\"," +
                                 $"\"text\":\"{text}\"," +
                                 $"\"picUrl\":\"{imageUrl}\"," +
                                 $"\"messageUrl\":\"{contentUrl}\"}}}}");
        var returnJson = Post(JsonStringBuilder.ToString());
        Console.Write(returnJson);
    }


    /// <summary>
    ///     发送markdown类消息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="text">消息主体</param>
    /// <param name="atMobiles">@人员电话</param>
    /// <param name="isAtAll">是否@群所有成员</param>
    public static void SendMarkdown(string title, string text, string[] phoneNumbers, bool isAtAll)
    {
        var markdownMsg = new MarkdownMsg();
        markdownMsg.Markdown.Title = title;
        markdownMsg.Markdown.Content = text;
        markdownMsg.At.AtMobiles = phoneNumbers;
        markdownMsg.At.IsAtAll = isAtAll;
        var json = JsonConvert.SerializeObject(markdownMsg);
        Console.WriteLine(json);
        JsonStringBuilder.Clear();
        JsonStringBuilder.Append(json);
        var returnJson = Post(JsonStringBuilder.ToString());
        Console.Write(returnJson);
    }


    /// <summary>
    ///     整体发送ActionCard类型消息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="imageUrl">图片Url</param>
    /// <param name="texttitle">文本标题</param>
    /// <param name="text">文本</param>
    /// <param name="btnTitle">按钮标题</param>
    /// <param name="buttonUrl">按钮Url</param>
    /// <param name="btnOrientation">按钮排列     0-按钮竖直排列，1-按钮横向排列</param>
    public static void SendActionCard(string title, string imageUrl, string texttitle, string text, string btnTitle,
        string buttonUrl, string btnOrientation)
    {
        JsonStringBuilder.Clear();
        JsonStringBuilder.Append($"{{\"actionCard\":{{\"title\":\"{Keyword + title}\"," +
                                 $"\"text\":\"![screenshot]({imageUrl}) \\r\\n ### {texttitle} \\r\\n {text}\"," +
                                 $"\"btnOrientation\":\"{btnOrientation}\"," +
                                 $"\"singleTitle\":\"{btnTitle}\"," +
                                 $"\"singleURL\":\"{buttonUrl}\"}}," +
                                 $"\"msgtype\":\"actionCard\"}}");
        var returnJson = Post(JsonStringBuilder.ToString());
        //Console.Write(returnJson); 
    }

    /// <summary>
    ///     独立发送 ActionCard类型消息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="imageUrl">图片Url</param>
    /// <param name="texttitle">文本标题</param>
    /// <param name="text">文本</param>
    /// <param name="buttonUrl1">按钮1Url</param>
    /// <param name="buttonUrl2">按钮2Url</param>
    /// <param name="singleURL1">按钮1访问Url</param>
    /// <param name="singleURL2">按钮2访问Url</param>
    /// <param name="btnOrientation"></param>
    public static void SendActionCard(string title, string imageUrl, string texttitle, string text, string buttonUrl1,
        string buttonUrl2, string singleURL1, string singleURL2, string btnOrientation)
    {
        JsonStringBuilder.Clear();
        JsonStringBuilder.Append($"{{\"actionCard\":{{\"title\":\"{Keyword + title}\"," +
                                 $"\"text\":\"![screenshot]({imageUrl}) \\r\\n ### {texttitle} \\r\\n {text}\"," +
                                 $"\"btnOrientation\":\"{btnOrientation}\"," +
                                 $"\"btns\":[{{\"title\":\"{buttonUrl1}\"," +
                                 $"\"actionURL\":\"{singleURL1}\"}}," +
                                 $"{{\"title\":\"{buttonUrl2}\"," +
                                 $"\"actionURL\":\"{singleURL2}\"}}]}}," +
                                 $"\"msgtype\":\"actionCard\"}}");
        var returnJson = Post(JsonStringBuilder.ToString());
        //Console.Write(returnJson); 
    }


    /// <summary>
    ///     发送 FeedCard类型消息
    /// </summary>
    /// <param name="title1">标题 单条信息文本</param>
    /// <param name="messageUrl1">点击单条信息到跳转链接</param>
    /// <param name="imageUrl1">单条信息后面图片的URL</param>
    /// <param name="title2"></param>
    /// <param name="messageUrl2"></param>
    /// <param name="imageUrl2"></param>
    public static void SendFeedCard(string title1, string messageUrl1, string imageUrl1, string title2,
        string messageUrl2, string imageUrl2)
    {
        JsonStringBuilder.Clear();
        JsonStringBuilder.Append($"{{\"feedCard\":{{\"links\":[{{\"title\":\"{Keyword + title1}\"," +
                                 $"\"messageURL\":\"{messageUrl1}\"," +
                                 $"\"picURL\":\"{imageUrl1}\"}}," +
                                 $"{{\"title\":\"{title2}\"," +
                                 $"\"messageURL\":\"{messageUrl2}\"," +
                                 $"\"picURL\":\"{imageUrl2}\"}}]}}," +
                                 $"\"msgtype\":\"feedCard\"}}");
        var returnJson = Post(JsonStringBuilder.ToString());
    }
}