using System.ComponentModel;

namespace Core.Enums;

/// <summary>
/// 缺陷分类
/// </summary>
public enum DefectCategory
{
    [Description("A")]
    A = 1,
    [Description("B")]
    B = 2,
    [Description("C")]
    C = 3,
    [Description("D")]
    D = 4
}