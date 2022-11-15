using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_packaging_machine")]
[Comment("包装机")]
public class PackagingMachine: BaseDataEntity
{
    
}