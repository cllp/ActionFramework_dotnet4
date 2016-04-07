using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ActionFramework.Server.Data.Helpers
{
    public static class SqlHelper
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        /// <summary>
        /// Opens connection string configured in web.config.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetOpenConnection()
        {
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(connectionString);
            scsb.Pooling = true;
            SqlConnection conn = new SqlConnection(scsb.ConnectionString);
            conn.Open();
            return conn;
        }

        public static void SetIdentity<T>(IDbConnection connection, Action<T> setId)
        {
            dynamic identity = connection.Query("SELECT @@IDENTITY AS Id").Single();
            T newId = (T)identity.Id;
            setId(newId);
        }

        public static void SetIdentity<T>(IDbConnection connection, Action<T> setId, IDbTransaction transaction)
        {
            dynamic identity = connection.Query("SELECT @@IDENTITY AS Id", null, transaction).Single();
            T newId = (T)identity.Id;
            setId(newId);
        }
    }
}
