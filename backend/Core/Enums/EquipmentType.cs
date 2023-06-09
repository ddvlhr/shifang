using System.ComponentModel;

namespace Core.Enums;

/// <summary>
/// 测试台类型
/// </summary>
public enum EquipmentType
{
    [Description("RT综合测试台")]
    Rt = 1,
    [Description("MTS综合测试台")]
    Mts = 2,
    [Description("手工雪茄吸阻测试台")]
    SingleResistance = 3,
    [Description("未知")]
    None = 4,
}