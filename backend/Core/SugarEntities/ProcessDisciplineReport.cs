using System;
using Core.Entities;
using SqlSugar;

namespace Core.SugarEntities;

[SugarTable("t_process_discipline_report", TableDescription = "工艺纪律执行情况")]
public class ProcessDisciplineReport : SugarEntity
{
    [SugarColumn(ColumnName = "time", ColumnDescription = "检测时间")]
    public DateTime Time { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(DepartmentId))]
    public Department Department { get; set; }
    [SugarColumn(ColumnName = "department_id", ColumnDescription = "涉及部门")]
    public int DepartmentId { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(ClassId))]
    public DisciplineClass DisciplineClass { get; set; }
    [SugarColumn(ColumnName = "class_id", ColumnDescription = "纪律分类")]
    public int ClassId { get; set; }
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(ClauseId))]
    public DisciplineClause DisciplineClause { get; set; }
    [SugarColumn(ColumnName = "clause_id", ColumnDescription = "纪律条款")]
    public int ClauseId { get; set; }
    [SugarColumn(ColumnName = "description", ColumnDataType = "text", ColumnDescription = "现象描述")]
    public string Description { get; set; }
    [SugarColumn(ColumnName = "reward", Length = 128, ColumnDescription = "奖励情况")]

    public string Reward { get; set; }
    [SugarColumn(ColumnName = "punishment", Length = 128, ColumnDescription = "处罚情况")]
    public string Punishment { get; set; }
}