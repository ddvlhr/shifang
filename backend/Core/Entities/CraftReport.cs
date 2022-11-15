using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_craft_report")]
public class CraftReport : Entity
{
    [ForeignKey(nameof(GroupId))] public Group Group { get; set; }

    [Column("group_id")] public int GroupId { get; set; }

    [Column("order_no")] [MaxLength(128)] public string OrderNo { get; set; }

    [Column("score")] public double Score { get; set; }

    [Column("report_ret")] public ReportRet ReportRet { get; set; }

    [Column("remark")] [MaxLength(128)] public string Remark { get; set; }

    [Column("temperature")] public string Temperature { get; set; }

    [Column("control_situation")] public string ControlSituation { get; set; }

    [Column("log_order_no")]
    [MaxLength(128)]
    public string LogOrderNo { get; set; }
}