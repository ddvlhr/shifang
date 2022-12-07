using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Api.Settings.Impl;

public class WriteAbleOptions<T> : IWriteAbleOptions<T> where T : class, new()
{
    private readonly IConfigurationRoot _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly string _file;
    private readonly IOptionsMonitor<T> _options;
    private readonly string _section;

    public WriteAbleOptions(IWebHostEnvironment env,
        IOptionsMonitor<T> options,
        IConfigurationRoot configuration,
        string section,
        string file)
    {
        _env = env;
        _options = options;
        _configuration = configuration;
        _section = section;
        _file = file;
    }

    public T Value => _options.CurrentValue;

    public void Update(Action<T> applyChanges)
    {
        var fileProvider = _env.ContentRootFileProvider;
        var fileInfo = fileProvider.GetFileInfo(_file);
        var physicalPath = fileInfo.PhysicalPath;
        var jsonObj = JsonSerializer.Deserialize<JsonObject>(File.ReadAllText(physicalPath));
        if (jsonObj != null)
        {
            var sectionObj = jsonObj.TryGetPropertyValue(_section, out var section)
                ? section == null ? Value : JsonSerializer.Deserialize<T>(section.ToString())
                : Value;

            if (sectionObj != null)
            {
                applyChanges(sectionObj);
                jsonObj[_section] = JsonNode.Parse(JsonSerializer.Serialize(sectionObj));
            }
        }

        var serializeOptions = new JsonSerializerOptions()
        {
            // 保留 Json 格式
            WriteIndented = true
        };
        File.WriteAllText(physicalPath, JsonSerializer.Serialize(jsonObj, serializeOptions));
        _configuration.Reload();
    }

    public T Get(string name)
    {
        return _options.Get(name);
    }
}