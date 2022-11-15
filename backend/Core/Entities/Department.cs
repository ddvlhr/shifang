using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_department")]
[Comment("部门")]
public class Department: BaseDataEntity
{
    
}