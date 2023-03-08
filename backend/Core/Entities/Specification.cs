using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_specification")]
public class Specification : Entity
{
    [Column("name")] [MaxLength(128)] public string Name { get; set; }

    [Column("order_no")] [MaxLength(128)] public string OrderNo { get; set; }

    [ForeignKey(nameof(SpecificationTypeId))]
    public SpecificationType SpecificationType { get; set; }

    [Column("specification_type_id")] public int SpecificationTypeId { get; set; }

    [Column("remark")] [MaxLength(64)] public string Remark { get; set; }

    [Column("single_rules", TypeName = "text")]
    public string SingleRules { get; set; }

    [Column("mean_rules", TypeName = "text")]
    public string MeanRules { get; set; }

    [Column("sd_rules", TypeName = "text")]
    public string SdRules { get; set; }

    [Column("cpk_rules", TypeName = "text")]
    public string CpkRules { get; set; }

    [Column("cv_rules", TypeName = "text")]
    public string CvRules { get; set; }

    [Column("equipment_type")]
    public EquipmentType EquipmentType { get; set; }

    [Column("status")] public Status Status { get; set; }
}