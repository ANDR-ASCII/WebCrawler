using System;
using System.Collections.Generic;

namespace webcrawler.database
{
    class MySqlDbConnection : IDbConnection
    {
        MySql.Data.MySqlClient.MySqlConnection m_connection;
        string m_database;
        string m_password;
        string m_userName;
        string m_serverAddress;

        //============================================================

        public void setServerAddress(string serverAddress)
        {
            m_serverAddress = serverAddress;
        }

        public void setUserName(string userName)
        {
            m_userName = userName;
        }

        public void setUserPassword(string password)
        {
            m_password = password;
        }

        public void setDatabaseName(string database)
        {
            m_database = database;
        }

        public bool open()
        {
            try
            {
                m_connection = new MySql.Data.MySqlClient.MySqlConnection(
                    "server=" + m_serverAddress +
                    ";uid=" + m_userName +
                    ";pwd=" + m_password +
                    ";database=" + m_database);

                m_connection.Open();

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

        public bool close()
        {
            try
            {
                m_connection.Close();

                return true;
            }
            catch (MySql.Data.MySqlClient.MySqlException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return false;
        }

        public bool ping()
        {
            return m_connection != null && m_connection.Ping();
        }

        public void executeChangeQuery(string query)
        {
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(query, m_connection);
            command.ExecuteNonQuery();
        }

        public List<object>[] executeReadQuery(string query)
        {
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(query, m_connection);
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

        public ISqlTransaction beginTransaction()
        {
            return new MySqlTransaction(m_connection.BeginTransaction());
        }
    }
}
