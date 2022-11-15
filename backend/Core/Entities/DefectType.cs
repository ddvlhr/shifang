using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_defect_type")]
[Comment("缺陷类别")]
public class DefectType: BaseDataEntity
{
    
}