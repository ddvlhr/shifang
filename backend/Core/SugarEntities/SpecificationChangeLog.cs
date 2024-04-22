using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_specification_change_log")]
[Table("t_specification_change_log")]
public class SpecificationChangeLog: Entity
{
    [Navigate(NavigateType.OneToOne, nameof(SpecificationId))]
    [ForeignKey(nameof(SpecificationId))]
    public Specification Specification { get; set; }
    [SugarColumn(ColumnName = "specification_id")]
    [Column("specification_id")]
    public int SpecificationId { get; set; }
    [SugarColumn(ColumnName = "before", ColumnDataType = "text")]
    [Column("before", TypeName = "text")]
    public string Before { get; set; }
    [SugarColumn(ColumnName = "after", ColumnDataType = "text")]
    [Column("after", TypeName = "text")]
    public string After { get; set; }
    
    [SugarColumn(ColumnName = "change_id", IsNullable = true)]
    [Column("change_id")]
    public int? ChangeId { get; set; }
    
    [SugarColumn(ColumnName = "change_by")]
    [Column("change_by")]
    public string ChangeBy { get; set; }
}
