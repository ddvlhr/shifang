using System.ComponentModel;

namespace Core.Enums;
/// <summary>
/// 按钮类型
/// </summary>
public enum ButtonType
{
    [Description("default")]
    Default = 1,
    [Description("primary")]
    Primary = 2,
    [Description("success")]
    Success = 3,
    [Description("danger")]
    Danger = 4,
    [Description("warning")]
    Warning = 5
}