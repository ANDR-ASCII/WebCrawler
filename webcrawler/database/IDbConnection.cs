using System.Data;

namespace webcrawler.database
{
    interface IDbConnection
    {
        void SetServerAddress(string serverAddress);
        void SetUserName(string userName);
        void SetUserPassword(string password);
        void SetDatabaseName(string database);
        void Open();
        void Close();
        bool Ping();
        DataTable GetSchema(string name);
    }
}
