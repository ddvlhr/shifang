using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_process_discipline_report")]
[Comment("工艺纪律执行情况")]
public class ProcessDisciplineReport: Entity
{
    [Column("time")]
    [Comment("检测时间")]
    public DateTime Time { get; set; }
    [ForeignKey(nameof(DepartmentId))]
    public Department Department { get; set; }

    [Column("department_id")]
    [Comment("涉及部门")]
    public int DepartmentId { get; set; }
    [Column("description", TypeName = "text")]
    [Comment("现象描述")]
    public string Description { get; set; }
    [Column("reward")]
    [MaxLength(128)]
    [Comment("奖励情况")]
    public string Reward { get; set; }
    [Column("punishment")]
    [MaxLength(128)]
    [Comment("处罚情况")]
    public string Punishment { get; set; }
}