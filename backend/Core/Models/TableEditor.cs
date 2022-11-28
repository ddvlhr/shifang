using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Models;

public class TableEditor
{
    [JsonProperty("label")]
    public string Label { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("attrs")]
    public TableEditorAttr Attrs { get; set; }

    public class TableEditorAttr
    {
        [JsonProperty("isShowDelete")]
        public bool IsShowDelete { get; set; }
        [JsonProperty("rules")]
        public object Rules { get; set; }
        [JsonProperty("tableAttrs")]
        public Dictionary<string, object> TableAttrs { get; set; }
        [JsonProperty("columns")]
        public IEnumerable<ColumnAttr> Columns { get; set; }
    }

    public class ColumnAttr
    {
        [JsonProperty("prop")]
        public string Prop { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("width")]
        public string Width { get; set; }
        [JsonProperty("content")]
        public Dictionary<string, object> Content { get; set; }
    }
}