using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public abstract class Entity
{
    public static readonly DateTime EntityInitialDate = new(1997, 7, 18, 0, 0, 0, DateTimeKind.Utc);

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("created_at_utc")] public DateTime CreatedAtUtc { get; set; } = EntityInitialDate;

    [Column("modified_at_utc")] public DateTime ModifiedAtUtc { get; set; } = EntityInitialDate;
}

public enum Status
{
    [Description("启用")]
    Enabled = 0,
    [Description("停用")]
    Disabled = 1
}

public enum OnlineStatus
{
    [Description("在线")]
    Online = 1,
    [Description("离线")]
    Offline = 0
}

public enum FunctionPosition
{
    Line = 1,
    Top = 2
}

public enum IndicatorProject
{
    None = 0,

    /// <summary>
    ///     外观指标
    /// </summary>
    Appearance = 1,

    /// <summary>
    ///     物理指标
    /// </summary>
    Measure = 2,

    /// <summary>
    ///     化学指标
    /// </summary>
    Chemistry = 3,

    /// <summary>
    ///     工艺指标
    /// </summary>
    Craft = 4
}