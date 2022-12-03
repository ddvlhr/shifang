using System;
using SqlSugar;

namespace Infrastructure.DataBase;

public interface IDbContext
{
    /// <summary>
    /// 操作数据库对象
    /// </summary>
    public SqlSugarClient Db { get; }
    /// <summary>
    /// 创建数据表
    /// </summary>
    /// <param name="backup">是否备份</param>
    /// <param name="stringDefaultLength">string类型映射的长度</param>
    /// <param name="types">要创建的数据表</param>
    public void CreateTable(bool backup = false, int stringDefaultLength = 64, params Type[] types);
    /// <summary>
    /// 创建表
    /// </summary>
    /// <param name="backup">是否备份</param>
    /// <param name="stringDefaultLength">string类型映射的长度</param>
    public void CreateTable(bool backup = false, int stringDefaultLength = 64);
}