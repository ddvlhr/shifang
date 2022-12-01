using Core.Entities;
using SqlSugar;

namespace Core.SugarEntities;

public abstract class BaseData: SugarEntity
{   
    [SugarColumn(ColumnName = "name", Length = 128)]
    public string Name { get; set; }
    [SugarColumn(ColumnName = "status")]
    public Status Status { get; set; }
}