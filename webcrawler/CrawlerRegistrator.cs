using System;
using System.Collections.Generic;
using webcrawler.database;

namespace webcrawler
{
    class CrawlerRegistrator
    {
        private IDbConnection m_connection;
        private uint m_crawlerId;

        public uint CrawlerId
        {
            get { return m_crawlerId; }
        }

        public IDbConnection DbConnection
        {
            get { return m_connection; }
        }

        public CrawlerRegistrator(DatabaseType dbType, string dbName, string server, string user, string password)
        {
            m_connection = ConnectionFactory.CreateConnection(DatabaseType.DatabaseMySql);
            m_connection.SetDatabaseName(dbName);
            m_connection.SetServerAddress(server);
            m_connection.SetUserName(user);
            m_connection.SetUserPassword(password);
            m_connection.Open();

            if (!RegistrateCrawlerId(m_connection))
            {
                throw new Exception("Some crawler is already registered on this machine");
            }

            m_crawlerId = GetCrawlerId(m_connection);

            AppDomain.CurrentDomain.ProcessExit += UnregisterCrawler;
        }

        uint GetCrawlerId(IDbConnection connection)
        {
            Dictionary<string, object>[] data = connection.ExecuteReadQuery("SELECT id FROM crawler_id WHERE mac_address='" + Helpers.GetMacAddress() + "'", "id");
            return (uint)data[0]["id"];
        }
        bool RegistrateCrawlerId(IDbConnection connection)
        {
            ISqlTransaction transaction = connection.BeginTransaction();

            string selectQuery = "SELECT COUNT(*) FROM crawler_id WHERE mac_address='" + Helpers.GetMacAddress() + "'";
            List<object>[] data = connection.ExecuteReadQuery(selectQuery);

            bool hasNoCrawlersWithThisMac = data.Length != 0 && data[0].Count > 0 && (long)data[0][0] == 0;

            if (hasNoCrawlersWithThisMac)
            {
                connection.ExecuteNonQuery("INSERT INTO crawler_id(mac_address) VALUES('" + Helpers.GetMacAddress() + "')");
                data = connection.ExecuteReadQuery(selectQuery);

                bool hasOnlyOneCrawlerWithThisMac = data.Length != 0 && data[0].Count > 0 && (long)data[0][0] == 1;

                if (!hasOnlyOneCrawlerWithThisMac)
                {
                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();
                return true;
            }

            transaction.Rollback();
            return false;
        }
        void RemoveCrawlerId(IDbConnection connection)
        {
            connection.ExecuteNonQuery("DELETE FROM crawler_id WHERE mac_address='" + Helpers.GetMacAddress() + "'");
        }
        void UnregisterCrawler(object sender, EventArgs e)
        {
            RemoveCrawlerId(m_connection);
        }
    }
}
