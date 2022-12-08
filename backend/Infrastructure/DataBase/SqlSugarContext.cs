using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Core.SugarEntities;
using Infrastructure.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Infrastructure.DataBase;

public static class SqlSugarContext
{
    public static void AddSqlSugarSetup(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = AppConfigurationHelper.GetSection<Core.Models.ShiFangSettings>("ShiFangSettings");
        var connectionString = AppConfigurationHelper.Configuration.GetConnectionString(settings.DataSource);
        var sqlSugar = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = connectionString,
            DbType = DbType.MySqlConnector,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            ConfigureExternalServices = new ConfigureExternalServices
            {
                EntityService = (c, p) =>
                {
                    // int?  decimal?这种 isnullable=true
                    if (c.PropertyType.IsGenericType &&
                c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        p.IsNullable = true;
                    }
                    else if (c.PropertyType == typeof(string) &&
                         c.GetCustomAttribute<RequiredAttribute>() == null)
                    { //string类型如果没有Required isnullable=true
                        p.IsNullable = true;
                    }
                }
            }
        });
        sqlSugar.CodeFirst.InitTables<ShiFangSettings, DisciplineClause, DisciplineClass, ProcessDisciplineReport>();
        sqlSugar.Aop.OnLogExecuting = (sql, pars) => { Console.WriteLine(sql + "\r"); };
        services.AddSingleton<ISqlSugarClient>(sqlSugar);
    }
}