using Chloe.Infrastructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.MySql
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        string _connString;
        public MySqlConnectionFactory(string connString, string dataBaseName)
        {
            this._connString = string.Format(connString, dataBaseName);
        }
        public IDbConnection CreateConnection()
        {
            MySqlConnection conn = new MySqlConnection(this._connString);
            return conn;
        }
    }
}
