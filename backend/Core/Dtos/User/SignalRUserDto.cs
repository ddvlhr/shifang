using Newtonsoft.Json;

namespace Core.Dtos.User;

public class SignalRUserDto
{
    public string ConnectionId { get; set; }
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("userName")]
    public string UserName { get; set; }
    public string Token { get; set; }
    [JsonProperty("machine")]
    public int Machine { get; set; }
}