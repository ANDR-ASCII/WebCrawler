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

        public void SetServerAddress(string serverAddress)
        {
            m_serverAddress = serverAddress;
        }

        public void SetUserName(string userName)
        {
            m_userName = userName;
        }

        public void SetUserPassword(string password)
        {
            m_password = password;
        }

        public void SetDatabaseName(string database)
        {
            m_database = database;
        }

        public bool Open()
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

        public bool Close()
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

        public bool Ping()
        {
            return m_connection != null && m_connection.Ping();
        }

        public int ExecuteNonQuery(string query, params SqlCommandParameter[] sqlCommandParameters)
        {
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(query, m_connection);

            foreach (SqlCommandParameter sqlCommandParameter in sqlCommandParameters)
            {
                command.Parameters.Add(sqlCommandParameter.parameterName, MapProcedureArgumentType(sqlCommandParameter.type)).Value = sqlCommandParameter.value;
            }

            return command.ExecuteNonQuery();
        }

        public int ExecuteStoredProcedure(
            string procedureName,
            string query,
            Dictionary<string, (ProcedureArgumentType type, bool isOutput)> procedureParameters)
        {
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand();

            // drop procedure if needed
            command.Connection = m_connection;
            command.CommandText = "DROP PROCEDURE IF EXISTS " + procedureName;
            command.ExecuteNonQuery();

            // creating procedure
            command.CommandText = query;
            command.ExecuteNonQuery();

            // execute procedure
            command.CommandText = procedureName;
            command.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (KeyValuePair<string, (ProcedureArgumentType type, bool isOutput)> entry in procedureParameters)
            {
                var (sqlProcedureArgumentType, isOutput) = entry.Value;
                command.Parameters.AddWithValue("@" + entry.Key, MapProcedureArgumentType(sqlProcedureArgumentType));

                command.Parameters["@" + entry.Key].Direction = isOutput ?
                    System.Data.ParameterDirection.Output :
                    System.Data.ParameterDirection.Input;
            }

            return command.ExecuteNonQuery();
        }

        public List<object>[] ExecuteReadQuery(string query)
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

        public Dictionary<string, object>[] ExecuteReadQuery(string query, params string[] columns)
        {
            MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(query, m_connection);
            MySql.Data.MySqlClient.MySqlDataReader dataReader = command.ExecuteReader();

            Dictionary<string, object>[] result = new Dictionary<string, object>[dataReader.FieldCount];

            if (dataReader.FieldCount != columns.Length)
            {
                throw new ArgumentException("Column count must be equal to extraction from table column count");
            }

            for (int i = 0; i < dataReader.FieldCount; ++i)
            {
                result[i] = new Dictionary<string, object>();
            }

            while (dataReader.Read())
            {
                for (int i = 0; i < dataReader.FieldCount; ++i)
                {
                    result[i].Add(columns[i], dataReader[columns[i]]);
                }
            }

            dataReader.Close();
            return result;
        }

        public ISqlTransaction BeginTransaction()
        {
            return new MySqlTransaction(m_connection.BeginTransaction());
        }

        MySql.Data.MySqlClient.MySqlDbType MapProcedureArgumentType(ProcedureArgumentType type)
        {
            switch (type)
            {
                case ProcedureArgumentType.Decimal:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Decimal;
                }
                case ProcedureArgumentType.Byte:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Byte;
                }
                case ProcedureArgumentType.Int16:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Int16;
                }
                case ProcedureArgumentType.Int32:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Int32;
                }
                case ProcedureArgumentType.Float:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Float;
                }
                case ProcedureArgumentType.Double:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Double;
                }
                case ProcedureArgumentType.Timestamp:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Timestamp;
                }
                case ProcedureArgumentType.Int64:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Int64;
                }
                case ProcedureArgumentType.Int24:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Int24;
                }
                case ProcedureArgumentType.Date:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Date;
                }
                case ProcedureArgumentType.Time:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Time;
                }
                case ProcedureArgumentType.DateTime:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.DateTime;
                }
                case ProcedureArgumentType.Year:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Year;
                }
                case ProcedureArgumentType.Newdate:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Newdate;
                }
                case ProcedureArgumentType.VarString:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.VarString;
                }
                case ProcedureArgumentType.Bit:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Bit;
                }
                case ProcedureArgumentType.JSON:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.JSON;
                }
                case ProcedureArgumentType.NewDecimal:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.NewDecimal;
                }
                case ProcedureArgumentType.Enum:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Enum;
                }
                case ProcedureArgumentType.Set:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Set;
                }
                case ProcedureArgumentType.TinyBlob:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.TinyBlob;
                }
                case ProcedureArgumentType.MediumBlob:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.MediumBlob;
                }
                case ProcedureArgumentType.LongBlob:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.LongBlob;
                }
                case ProcedureArgumentType.Blob:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Blob;
                }
                case ProcedureArgumentType.VarChar:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.VarChar;
                }
                case ProcedureArgumentType.String:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.String;
                }
                case ProcedureArgumentType.Geometry:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Geometry;
                }
                case ProcedureArgumentType.UInt16:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.UInt16;
                }
                case ProcedureArgumentType.UInt32:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.UInt32;
                }
                case ProcedureArgumentType.UInt64:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.UInt64;
                }
                case ProcedureArgumentType.UInt24:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.UInt24;
                }
                case ProcedureArgumentType.Binary:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Binary;
                }
                case ProcedureArgumentType.VarBinary:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.VarBinary;
                }
                case ProcedureArgumentType.TinyText:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.TinyText;
                }
                case ProcedureArgumentType.MediumText:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.MediumText;
                }
                case ProcedureArgumentType.LongText:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.LongText;
                }
                case ProcedureArgumentType.Text:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Text;
                }
                case ProcedureArgumentType.Guid:
                {
                    return MySql.Data.MySqlClient.MySqlDbType.Guid;
                }
            }

            throw new Exception("Passed invalid procedure argument type");
        }
    }
}
