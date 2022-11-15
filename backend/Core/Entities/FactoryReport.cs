using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_factory_report")]
public class FactoryReport : Entity
{
    [Column("test_date")] public DateTime TestDate { get; set; }

    [ForeignKey(nameof(GroupId))] public Group Group { get; set; }

    [Column("group_id")] public int GroupId { get; set; }

    /// <summary>
    ///     滤棒出厂报表外规需要多组数据
    /// </summary>
    [Column("group_ids")]
    [MaxLength(128)]
    public string GroupIds { get; set; }

    [ForeignKey(nameof(SpecificationId))] public Specification Specification { get; set; }

    [Column("specification_id")] public int SpecificationId { get; set; }

    [Column("order_no")] [MaxLength(128)] public string OrderNo { get; set; }

    [Column("manufacturer_place")]
    [MaxLength(128)]
    public string ManufacturerPlace { get; set; }

    [Column("test_method")]
    [MaxLength(64)]
    public string TestMethod { get; set; }

    [Column("result")] [MaxLength(256)] public string Result { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; }

    [Column("user_id")] public int UserId { get; set; }

    [Column("report_ret")] public ReportRet ReportRet { get; set; }

    [Column("report_type")] public ReportType ReportType { get; set; }

    [Column("chemical_data_id")] public int ChemicalDataId { get; set; }

    [Column("count")] [MaxLength(64)] public string Count { get; set; }

    [Column("count_in_box")] public int CountInBox { get; set; }

    [Column("audit_user")] [MaxLength(64)] public string AuditUser { get; set; }
}

public enum ReportType
{
    Self = 1,
    Cooperation = 100
}