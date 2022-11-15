using Newtonsoft.Json;

namespace Infrastructure.Response;

public class Response
{
    public Response(int code, string msg, object data)
    {
        Data = data;
        Meta = new MetaData
        {
            Code = code,
            Message = msg
        };
    }

    [JsonProperty("data")] public object Data { get; set; }

    [JsonProperty("meta")] public MetaData Meta { get; set; }

    public class MetaData
    {
        [JsonProperty("message")] public string Message { get; set; }

        [JsonProperty("code")] public int Code { get; set; }
    }
}