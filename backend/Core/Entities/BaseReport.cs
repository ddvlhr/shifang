using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public abstract class BaseReport : Entity
{
    /// <summary>
    ///     测量组ID
    /// </summary>
    [Column("group_id")]
    public int GroupId { get; set; }

    [ForeignKey(nameof(GroupId))] public Group Group { get; set; }

    /// <summary>
    ///     水分
    /// </summary>
    [Column("water")]
    [MaxLength(64)]
    public string Water { get; set; }

    /// <summary>
    ///     湿度
    /// </summary>
    [Column("humidity")]
    [MaxLength(64)]
    public string Humidity { get; set; }

    /// <summary>
    ///     温度
    /// </summary>
    [Column("temperature")]
    [MaxLength(64)]
    public string Temperature { get; set; }

    /// <summary>
    ///     物理判定
    /// </summary>
    [Column("phy_ret")]
    public ReportRet PhyRet { get; set; }

    /// <summary>
    ///     物理判定描述
    /// </summary>
    [Column("phy_ret_des")]
    [MaxLength(256)]
    public string PhyRetDes { get; set; }

    /// <summary>
    ///     物理指标扣分
    /// </summary>
    [Column("phy_ret_deduction")]
    public int PhyRetDeduction { get; set; }

    /// <summary>
    ///     备注
    /// </summary>
    [Column("remark")]
    [MaxLength(128)]
    public string Remark { get; set; }

    /// <summary>
    ///     最终判定
    /// </summary>
    [Column("final_ret")]
    public ReportRet FinalRet { get; set; }

    [Column("total")] public double Total { get; set; }
}

public enum ReportRet
{
    未完成 = 0,
    不合格 = 100,
    合格 = 1
}