using System.Data;
using MySql.Data.MySqlClient;

namespace webcrawler.database
{
    class MySqlDbConnection : IDbConnection
    {
        public void SetServerAddress(string serverAddress)
        {
            _serverAddress = serverAddress;
        }

        public void SetUserName(string userName)
        {
            _userName = userName;
        }

        public void SetUserPassword(string password)
        {
            _password = password;
        }

        public void SetDatabaseName(string database)
        {
            _database = database;
        }

        public void Open()
        {
            _connection = new MySqlConnection("server=" + _serverAddress + ";uid=" + _userName + ";pwd=" + _password + ";database=" + _database);
            _connection.Open();
        }

        public void Close()
        {
            _connection.Close();
        }

        public bool Ping()
        {
            return _connection.Ping();
        }

        public DataTable GetSchema(string name)
        {
            return _connection.GetSchema(name);
        }

        //////////////////////////////////////////////////////////////////////////////

        private MySqlConnection _connection;
        private string _database;
        private string _password;
        private string _userName;
        private string _serverAddress;
    }
}
