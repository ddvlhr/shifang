using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

/// <summary>
///     运行日志
/// </summary>
[Table("t_log")]
public class Log : Entity
{
    /// <summary>
    ///     描述
    /// </summary>
    [Column("description")]
    [MaxLength(256)]
    public string Description { get; set; }

    /// <summary>
    ///     操作人员
    /// </summary>
    [Column("user")]
    [MaxLength(64)]
    public string User { get; set; }

    /// <summary>
    ///     操作
    /// </summary>
    [Column("operating")]
    [MaxLength(64)]
    public string Operating { get; set; }
}