using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;
[Table("t_calibration")]
public class Calibration: Entity
{
    [Column("time")]
    [Comment("标定验证时间")]
    public DateTime Time { get; set; }
    [Column("instance")]
    [MaxLength(64)]
    [Comment("仪器名称")]
    public string Instance { get; set; }
    [Column("equipment_type")]
    [Comment("测试台类型")]
    public EquipmentType EquipmentType { get; set; }
    [Column("operation")]
    [MaxLength(64)]
    [Comment("操作")]
    public string Operation { get; set; }
    [Column("unit")]
    [MaxLength(64)]
    [Comment("单元")]
    public string Unit { get; set; }
    [Column("unit_type")]
    [MaxLength(64)]
    [Comment("单位")]
    public string UnitType { get; set; }
    [Column("result_code")]
    [Comment("结果")]
    public int ResultCode { get; set; }
    [Column("description")]
    [MaxLength(256)]
    [Comment("描述")]
    public string Description { get; set; }
    [Column("temperature")]
    [MaxLength(64)]
    [Comment("温度")]
    public string Temperature { get; set; }
    [Column("humidity")]
    [MaxLength(64)]
    [Comment("湿度")]
    public string Humidity { get; set; }
}