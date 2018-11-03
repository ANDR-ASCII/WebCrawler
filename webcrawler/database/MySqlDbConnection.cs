using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

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

        public bool Open()
        {
            try
            {
                _connection = new MySqlConnection("server=" + _serverAddress + ";uid=" + _userName + ";pwd=" + _password + ";database=" + _database);
                _connection.Open();

                return true;
            }
            catch(MySqlException exception)
            {
                switch (exception.Number)
                {
                    case 0:
                    {
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
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
            catch (MySqlException exception)
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
            MySqlCommand command = new MySqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public List<object>[] ExecuteReadQuery(string query)
        {
            MySqlCommand command = new MySqlCommand(query, _connection);
            MySqlDataReader dataReader = command.ExecuteReader();

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
