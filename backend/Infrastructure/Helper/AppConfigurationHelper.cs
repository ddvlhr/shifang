using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Infrastructure.Helper
{
    public class AppConfigurationHelper
    {
        public static IConfiguration Configuration { get; set; }
        static AppConfigurationHelper()
        {
            Configuration = new ConfigurationBuilder().Add(new JsonConfigurationSource { Path = "appsettings.shifang.json", Optional = false, ReloadOnChange = true }).Build();
        }

        public static T GetSection<T>(string key = null) where T : class, new()
        {
            var appConfig = new T();
            if (string.IsNullOrEmpty(key))
            {
                Configuration.Bind(appConfig);
            }
            else
            {
                Configuration.GetSection(key).Bind(appConfig);
            }
            return appConfig;
        }
    }
}