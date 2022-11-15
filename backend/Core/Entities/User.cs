using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_user")]
public class User : Entity
{
    [Column("user_name")] [MaxLength(64)] public string UserName { get; set; }

    [Column("nick_name")] [MaxLength(64)] public string NickName { get; set; }

    [Column("hashed_password")]
    [MaxLength(64)]
    public string HashedPassword { get; set; }

    [Column("status")] public Status Status { get; set; }
}