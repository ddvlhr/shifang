using System.ComponentModel;

namespace Core.Enums;

public enum DepartmentType
{
    [Description("不限")]
    All = 0,
    [Description("卷包")]
    Cigarette = 1,
    [Description("手工")]
    Cigar = 2,
}