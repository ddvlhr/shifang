using System;
using System.Linq;
using Core.SugarEntities;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace Infrastructure.DataBase;

/// <summary>
/// 数据库上下文
/// </summary>
public class AppDbContext: IDbContext
{
    private readonly IConfiguration _configuration;
    
    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        // 打印日志
        Db.Aop.OnLogExecuting = (sql, pars) =>
        {
            Console.WriteLine(sql + "\r\n" +
                              Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
        };
    }

    public AppDbContext()
    {
        
    }
    
    /// <summary>
    /// 数据库操作对象
    /// </summary>
    public SqlSugarClient Db =>
        new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = _configuration.GetConnectionString("local"),
            DbType = DbType.MySql,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });

    /// <summary>
    /// 创建数据表
    /// </summary>
    /// <param name="backup">是否备份</param>
    /// <param name="stringDefaultLength">string类型映射的长度</param>
    /// <param name="types">要创建的数据表</param>
    public void CreateTable(bool backup = false, int stringDefaultLength = 64, params Type[] types)
    {
        Db.CodeFirst.SetStringDefaultLength(stringDefaultLength);

        if (backup)
        {
            Db.CodeFirst.BackupTable().InitTables(types);
        }
        else
        {
            Db.CodeFirst.InitTables(types);
        }
    }
    
    /// <summary>
    /// 创建表
    /// </summary>
    /// <param name="backup">是否备份</param>
    /// <param name="stringDefaultLength">string类型映射的长度</param>
    public void CreateTable(bool backup = false, int stringDefaultLength = 64)
    {
        Db.CodeFirst.SetStringDefaultLength(stringDefaultLength);
        
        var entityType = typeof(SugarEntity);
        var types = entityType
            .Assembly
            .GetExportedTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface && entityType.IsAssignableFrom(type))
            .ToArray();
        if (backup)
        {
            Db.CodeFirst.BackupTable().InitTables(types);
        }
        else
        {
            Db.CodeFirst.InitTables(types);
        }
    }
}