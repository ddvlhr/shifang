using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_data")]
public class Data : BaseData
{
    [ForeignKey(nameof(GroupId))] public Group Group { get; set; }
}