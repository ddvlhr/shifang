using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_material_report")]
public class MaterialReport : Entity
{
    [Column("test_date")] public DateTime TestDate { get; set; }

    [ForeignKey(nameof(GroupId))] public Group Group { get; set; }

    [Column("group_id")] public int GroupId { get; set; }

    [Column("order_no")] [MaxLength(128)] public string OrderNo { get; set; }

    [Column("manufacturer_id")] public int ManufacturerId { get; set; }

    [Column("manufacturer_name")]
    [MaxLength(128)]
    public string ManufacturerName { get; set; }

    [ForeignKey(nameof(SpecificationTypeId))]
    public SpecificationType SpecificationType { get; set; }

    [Column("specification_id")] public int SpecificationTypeId { get; set; }

    [Column("sample_place")]
    [MaxLength(64)]
    public string SamplePlace { get; set; }

    [Column("sample_count")]
    [MaxLength(64)]
    public string SampleCount { get; set; }

    [Column("unit")] [MaxLength(64)] public string Unit { get; set; }

    [Column("description", TypeName = "text")]
    public string Description { get; set; }

    [Column("other_desc", TypeName = "text")]
    public string OtherDesc { get; set; }

    [Column("result")] [MaxLength(256)] public string Result { get; set; }

    [ForeignKey(nameof(Checker))] public User User { get; set; }

    [Column("checker")] public int Checker { get; set; }

    [Column("report_ret")] public ReportRet ReportRet { get; set; }

    [Column("temperature")]
    [MaxLength(64)]
    public string Temperature { get; set; }

    [Column("humidity")] [MaxLength(64)] public string Humidity { get; set; }
}