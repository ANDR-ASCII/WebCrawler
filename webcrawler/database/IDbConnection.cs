using System.Collections.Generic;

namespace webcrawler.database
{
    interface IDbConnection
    {
        void SetServerAddress(string serverAddress);
        void SetUserName(string userName);
        void SetUserPassword(string password);
        void SetDatabaseName(string database);
        bool Open();
        bool Close();
        bool Ping();

        void ExecuteChangeQuery(string query);
        List<object>[] ExecuteReadQuery(string query);

        ISqlTransaction BeginTransaction();
    }
}
