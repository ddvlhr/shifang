using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_menu")]
public class Menu : Entity
{
    [Column("name")] [MaxLength(64)] public string Name { get; set; }

    [Column("url")] [MaxLength(64)] public string Url { get; set; }

    [Column("icon")] [MaxLength(64)] public string Icon { get; set; }

    [Column("level")] public int Level { get; set; }

    [Column("status")] public Status Status { get; set; }
}