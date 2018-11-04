using System;

namespace webcrawler.database
{
    enum DatabaseType
    {
        DatabaseMySql,
        DatabasePostgreSql,
        DatabaseSqlite
    }

    class ConnectionFactory
    {
        public static database.IDbConnection CreateConnection(DatabaseType type)
        {
            switch (type)
            {
                case DatabaseType.DatabaseMySql:
                {
                    return new database.MySqlDbConnection();
                }
                case DatabaseType.DatabasePostgreSql:
                case DatabaseType.DatabaseSqlite:
                {
                    throw new Exception("This database type is not implemented");
                }
            }

            throw new Exception("Unknown database type");
        }
    }
}
