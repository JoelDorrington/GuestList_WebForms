using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication1.Services
{
    public interface ISqlConnectionWrapper : IDisposable
    {
        SqlCommandWrapper CreateCommand(string commandText);
        void Open();
    }

    public interface ISqlCommandWrapper : IDisposable
    {
        string CommandText { get; set; }
        void ExecuteNonQuery();
        void AddParameter(string name, object value);
    }
    public class SqlConnectionWrapper : ISqlConnectionWrapper
    {
        private readonly SqlConnection _connection;

        public SqlConnectionWrapper()
        {
            _connection = null;
        }
        public SqlConnectionWrapper(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }
        public virtual SqlCommandWrapper CreateCommand(string commandText)
        {
            if (_connection == null) return null;
            return new SqlCommandWrapper(commandText, _connection);
        }

        public virtual void Open()
        {
            if (_connection == null) return;
            _connection.Open();
        }

        public virtual void Dispose()
        {
            if (_connection == null) return;
            _connection.Dispose();
        }
    }
    public class SqlCommandWrapper : ISqlCommandWrapper
    {
        private readonly SqlCommand _command;

        public SqlCommandWrapper()
        {
            _command = null;
        }
        public SqlCommandWrapper(string commandText, SqlConnection connection)
        {
            _command = new SqlCommand(commandText, connection);
        }

        public string CommandText
        {
            get => _command?.CommandText;
            set {
                if (_command == null) return;
                _command.CommandText = value;
            }
        }

        public virtual void ExecuteNonQuery()
        {
            if (_command == null) return;
            _command.ExecuteNonQuery();
        }

        public virtual SqlDataReader ExecuteReader()
        {
            if (_command == null) throw new Exception("_command is null");
            return _command.ExecuteReader();
        }

        public virtual void AddParameter(string name, object value)
        {
            if(_command == null) return;
            _command.Parameters.AddWithValue(name, value);
        }

        public virtual void Dispose()
        {
            if (_command == null) return;
            _command.Dispose();
        }
    }
}