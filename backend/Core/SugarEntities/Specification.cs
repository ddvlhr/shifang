using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities;
using Core.Enums;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_specification")]
public class Specification: SugarEntity
{
    [SugarColumn(ColumnName = "name", Length = 128)]
    public string Name { get; set; }
    [SugarColumn(ColumnName = "order_no", Length = 128)]
    public string OrderNo { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(SpecificationTypeId))]
    public SpecificationType SpecificationType { get; set; }
    [SugarColumn(ColumnName = "specification_type_id")]
    public int SpecificationTypeId { get; set; }
    [SugarColumn(ColumnName = "remark", Length = 64)]
    public string Remark { get; set; }
    [SugarColumn(ColumnName = "single_rules", ColumnDataType = "text")]
    public string SingleRules { get; set; }
    [SugarColumn(ColumnName = "mean_rules", ColumnDataType = "text")]
    public string MeanRules { get; set; }
    [SugarColumn(ColumnName = "sd_rules", ColumnDataType = "text")]
    public string SdRules { get; set; }
    [SugarColumn(ColumnName = "cpk_rules", ColumnDataType = "text")]
    public string CpkRules { get; set; }
    [SugarColumn(ColumnName = "cv_rules", ColumnDataType = "text")]
    public string CvRules { get; set; }
    [SugarColumn(ColumnName = "status")]
    public Status Status { get; set; }
    [SugarColumn(ColumnName = "equipment_type")]
    public EquipmentType EquipmentType { get; set; }
}