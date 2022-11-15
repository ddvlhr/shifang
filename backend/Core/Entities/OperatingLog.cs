using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("t_operating_log")]
public class OperatingLog : Entity
{
    [Column("path")] [MaxLength(128)] public string Path { get; set; }

    [Column("request")] [MaxLength(512)] public string Request { get; set; }

    [Column("response")] [MaxLength(512)] public string Response { get; set; }

    [Column("description")]
    [MaxLength(128)]
    public string Description { get; set; }

    [Column("user_name")] [MaxLength(64)] public string UserName { get; set; }

    [Column("execution_time")]
    [MaxLength(64)]
    public string ExecutionTime { get; set; }
}