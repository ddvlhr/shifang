using System.Configuration;
using MySqlConnector;

namespace WebService
{
    public class DbHelper
    {
        public static MySqlConnection Conn()
        {
            var connectionStr = ConfigurationManager.ConnectionStrings["mysql"].ToString();
            var connection = new MySqlConnection(connectionStr);
            connection.Open();
            return connection;
        }
    }
}