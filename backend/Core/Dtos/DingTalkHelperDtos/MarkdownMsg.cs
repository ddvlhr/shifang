using Newtonsoft.Json;

namespace Core.Dtos.DingTalkHelperDtos;

public class MarkdownMsg
{
    [JsonProperty("msgtype")] public string MsgType { get; set; } = "markdown";

    [JsonProperty("markdown")] public MarkdownAttr Markdown { get; set; } = new();

    [JsonProperty("at")] public At At { get; set; } = new();

    public class MarkdownAttr
    {
        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("content")] public string Content { get; set; }
    }
}