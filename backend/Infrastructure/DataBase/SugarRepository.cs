using Core.Models;
using Infrastructure.Helper;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace Infrastructure.DataBase;

public class SugarRepository<T> : SimpleClient<T> where T : class, new()
{
    public SugarRepository(ISqlSugarClient context = null) : base(context)
    {
        if (context == null)
        {
            var settings = AppConfigurationHelper.GetSection<ShiFangSettings>("ShiFangSettings");
            var connectionString = AppConfigurationHelper.Configuration.GetConnectionString(settings.DataSource);
            base.Context = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = DbType.MySqlConnector,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
        }
    }
}