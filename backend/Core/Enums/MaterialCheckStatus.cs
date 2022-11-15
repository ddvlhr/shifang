using System.ComponentModel;

namespace Core.Enums;

public enum MaterialCheckStatus
{
    [Description("未检测")]
    Undetected = 1,
    [Description("已检测")]
    Detected = 2,
    [Description("驳回")]
    Rejected = 3,
    [Description("完成")]
    Done = 4
}