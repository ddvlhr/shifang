using System;
using SqlSugar;

namespace Core.Entities;

public abstract class SugarEntity
{
    public static readonly DateTime EntityInitialDate = new(1997, 7, 18, 0, 0, 0, DateTimeKind.Utc);
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    [SugarColumn(ColumnName = "created_at_utc")]
    public DateTime CreatedAtUtc { get; set; } = EntityInitialDate;
    [SugarColumn(ColumnName = "modified_at_utc")]
    public DateTime ModifiedAtUtc { get; set; } = EntityInitialDate;
}