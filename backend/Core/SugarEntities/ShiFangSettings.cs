using System;
using System.ComponentModel.DataAnnotations.Schema;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_shifang_settings", TableDescription = "什邡系统设置表")]
public class ShiFangSettings: SugarEntity    
{
    [SugarColumn(ColumnName = "time", ColumnDescription = "保存时间")]
    public DateTime Time { get; set; }
    [SugarColumn(ColumnName = "settings", ColumnDataType = "text", ColumnDescription = "系统设置信息")]
    public string Settings { get; set; }
}