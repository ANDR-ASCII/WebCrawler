using System.Collections.Generic;

namespace webcrawler.database
{
    interface IDbConnection
    {
        void setServerAddress(string serverAddress);
        void setUserName(string userName);
        void setUserPassword(string password);
        void setDatabaseName(string database);
        bool open();
        bool close();
        bool ping();

        void executeChangeQuery(string query);
        List<object>[] executeReadQuery(string query);

        ISqlTransaction beginTransaction();
    }
}
