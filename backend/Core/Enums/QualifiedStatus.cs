using System.ComponentModel;

namespace Core.Enums;

public enum QualifiedStatus
{
    [Description("未判定")]
    Undefined = 1,
    [Description("合格")]
    Qualified = 2,
    [Description("不合格")]
    UnQualified = 3,
}