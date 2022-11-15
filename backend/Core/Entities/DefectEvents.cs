using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_defect_events")]
[Comment("缺陷类别小项")]
public class DefectEvents: BaseDataEntity
{
    
}