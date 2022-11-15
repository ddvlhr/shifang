using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public abstract class BaseDataEntity : Entity
{
    [Column("name")] [MaxLength(128)] public string Name { get; set; }

    [Column("status")] public Status Status { get; set; }
}