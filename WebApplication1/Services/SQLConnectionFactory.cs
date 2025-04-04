using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Services
{
    public interface ISQLConnectionFactory
    {
        IDbConnection GetConnection();
    }
    public class SQLConnectionFactory : ISQLConnectionFactory
    {
        private readonly string _connectionString;

        public SQLConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SQLConnectionFactory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["GuestLogDb"].ConnectionString;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}