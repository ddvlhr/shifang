using System;
using Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Infrastructure.DataBase;

public static class SqlSugarContext
{
    public static void AddSqlSugarSetup(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlSugar = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = configuration.GetConnectionString("tencent"),
            DbType = DbType.MySqlConnector,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
        // sqlSugar.CodeFirst.InitTables<MetricalGroup, MetricalData>();
        sqlSugar.Aop.OnLogExecuting = (sql, pars) => { Console.WriteLine(sql + "\r"); };
        services.AddSingleton<ISqlSugarClient>(sqlSugar);
    }
}