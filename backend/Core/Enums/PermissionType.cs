using System.ComponentModel;

namespace Core.Enums;
/// <summary>
/// 权限类型
/// </summary>
public enum PermissionType
{
    [Description("菜单")]
    Menu = 1,
    [Description("按钮")]
    Button = 2
}