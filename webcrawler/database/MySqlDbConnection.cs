using System;
using System.Collections.Generic;

namespace webcrawler.database
{
    class MySqlDbConnection : IDbConnection
    {
        MySql.Data.MySqlClient.MySqlConnection _connection;
        string _database;
        string _password;
        string _userName;
        string _serverAddress;

        //============================================================

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

        public bool Open()
        {
            try
            {
                _connection = new MySql.Data.MySqlClient.MySqlConnection(
                    "server=" + _serverAddress +
                    ";uid=" + _userName +
                    ";pwd=" + _password +
                    ";database=" + _database);

                _connection.Open();

                return true;
            }
            catch(MySql.Data.MySqlClient.MySqlException exception)
            {
                switch (exception.Number)
                {
                    case 0:
                    {
                        Console.WriteLine("Cannot connect to server. Contact administrator");
                        break;
                    }

                    case 1045:
                    {
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                    }
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return false;
        }

        public bool Close()
        {
            try
            {
                _connection.Close();

                return true;
            }
            catch (MySql.Data.MySqlClient.MySqlException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return false;
        }

        public bool Ping()
        {
            return _connection != null && _connection.Ping();
        }

        public void ExecuteChangeQuery(string query)
        {
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public List<object>[] ExecuteReadQuery(string query)
        {
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(query, _connection);
            MySql.Data.MySqlClient.MySqlDataReader dataReader = command.ExecuteReader();

            List<object>[] result = new List<object>[dataReader.FieldCount];

            for (int i = 0; i < dataReader.FieldCount; ++i)
            {
                result[i] = new List<object>();
            }

            while (dataReader.Read())
            {
                for (int i = 0; i < dataReader.FieldCount; ++i)
                {
                    result[i].Add(dataReader.GetValue(i));
                }
            }

            dataReader.Close();
            return result;
        }

        public ISqlTransaction BeginTransaction()
        {
            return new MySqlTransaction(_connection.BeginTransaction());
        }
    }
}
