using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace FuYang.WebService
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