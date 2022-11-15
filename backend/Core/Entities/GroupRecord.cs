using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_group_record")]
public class GroupRecord : BaseGroup
{
    [Column("count")] public int Count { get; set; }
}