using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_function")]
public class Function : Entity
{
    [Column("name")] [MaxLength(64)] public string Name { get; set; }

    [Column("function_name")]
    [MaxLength(64)]
    public string FunctionName { get; set; }

    [Column("type")] [MaxLength(64)] public string Type { get; set; }

    [Column("menu_id")] public int MenuId { get; set; }

    [ForeignKey(nameof(MenuId))] public Menu Menu { get; set; }

    [Column("position")] public FunctionPosition Position { get; set; } = FunctionPosition.Top;

    [Column("status")] public Status Status { get; set; }
}