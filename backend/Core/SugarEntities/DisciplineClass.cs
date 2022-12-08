using Core.Entities;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_discipline_class", TableDescription = "纪律分类表")]
public class DisciplineClass : SugarEntity
{
    [SugarColumn(ColumnName = "name", Length = 256)]
    public string Name { get; set; }
    [SugarColumn(ColumnName = "status")]
    public Status Status { get; set; }
}