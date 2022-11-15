using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("t_permission")]
public class Permission: Entity
{
    [Column("name")]
    [MaxLength(64)]
    [Comment("权限名称")]
    public string Name { get; set; }
    [Column("permission_type")]
    [Comment("权限类型[1 菜单, 2 按钮]")]
    public PermissionType PermissionType { get; set; }
    // 菜单
    [Column("level")]
    [Comment("菜单层级")]
    public int Level { get; set; }
    [Column("path")]
    [MaxLength(128)]
    [Comment("菜单路径")]
    public string Path { get; set; }
    [Column("icon")]
    [MaxLength(128)]
    [Comment("图标")]
    public string Icon { get; set; }
    [Column("component")]
    [MaxLength(256)]
    [Comment("组件路径")]
    public string Component { get; set; }
    
    // 按钮
    [Column("function_name")]
    [MaxLength(128)]
    [Comment("按钮方法名称")]
    public string FunctionName { get; set; }
    [Column("button_type")]
    [Comment("按钮类型")]
    public ButtonType ButtonType { get; set; }
    [Column("root")]
    [Comment("菜单 Id")]
    public int Root { get; set; }
    [Column("button_position")]
    [Comment("按钮位置[1 顶部, 2 行内]")]
    public ButtonPosition ButtonPosition { get; set; }
    [Column("order")]
    [Comment("序号")]
    public int Order { get; set; }
    [Column("status")]
    [Comment("状态")]
    public Status Status { get; set; }
}