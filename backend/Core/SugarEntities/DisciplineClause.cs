using Core.Entities;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_discipline_clause", TableDescription = "纪律条款表")]
public class DisciplineClause : SugarEntity
{
    [SugarColumn(ColumnName = "name", Length = 256)]
    public string Name { get; set; }
    [SugarColumn(ColumnName = "status")]
    public Status Status { get; set; }
}