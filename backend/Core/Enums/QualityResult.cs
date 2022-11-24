using System.ComponentModel;

namespace Core.Enums;

/// <summary>
/// 合格状态
/// </summary>
public enum QualityResult
{
    [Description("优质品")]
    Quality,
    [Description("一等品")]
    Grade,
    [Description("不合格品")]
    Nonconforming,
    [Description("二等品")]
    Seconds
}