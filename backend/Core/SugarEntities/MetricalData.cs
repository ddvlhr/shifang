using System;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_metrical_data", TableDescription = "测量数据详情表")]
public class MetricalData: SugarEntity
{
    [SugarColumn(ColumnName = "group_id")]
    public int GroupId { get; set; }
    [SugarColumn(IsIgnore = true)]
    public MetricalGroup Group { get; set; }
    [SugarColumn(ColumnName = "test_time")]
    public DateTime TestTime { get; set; }
    [SugarColumn(ColumnName = "data", ColumnDataType = "text")]
    public string Data { get; set; }

    // 报表用
    [SugarColumn(ColumnName = "total")]
    public int Total { get; set; }
    [SugarColumn(ColumnName = "result", Length = 64)]
    public string Result { get; set; }
    [SugarColumn(ColumnName = "remark", Length = 64)]
    public string Remark { get; set; }
}