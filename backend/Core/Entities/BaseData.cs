using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public abstract class BaseData : Entity
{
    [Column("group_id")] public int GroupId { get; set; }

    [Column("test_time")] public DateTime TestTime { get; set; }

    [Column("data", TypeName = "text")] public string Data { get; set; }

    // 报表用
    [Column("total")] public int Total { get; set; }

    [Column("result")] [MaxLength(64)] public string Result { get; set; }

    [Column("remark")] [MaxLength(64)] public string Remark { get; set; }
}