using Newtonsoft.Json;

namespace Core.Dtos.DingTalkHelperDtos;

public class At
{
    [JsonProperty("atMobiles")] public string[] AtMobiles { get; set; }
    [JsonProperty("atUserIds")] public string[] AtUserIds { get; set; }

    [JsonProperty("isAtAll")] public bool IsAtAll { get; set; }
}