using Chloe.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Chloe.SqlServer
{
    public class DefaultDbConnectionFactory : IDbConnectionFactory
    {
        string _connString;
        public DefaultDbConnectionFactory(string connString, string dataBaseName)
        {
            Utils.CheckNull(connString, "connString");

            this._connString = string.Format(connString, dataBaseName);
        }
        public IDbConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(this._connString);
            return conn;
        }
    }
}
