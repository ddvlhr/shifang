using Newtonsoft.Json;

namespace Core.Dtos.DingTalkHelperDtos;

public class NormalMsg
{
    [JsonProperty("msgtype")] public string MsgType { get; set; } = "text";

    [JsonProperty("text")] public TextAttr Text { get; set; } = new();
    [JsonProperty("at")] public At At { get; set; } = new();

    public class TextAttr
    {
        [JsonProperty("content")] public string Content { get; set; }
    }
}