using System.Collections.Generic;

namespace webcrawler.loader
{
    class DataProvider
    {
        database.IDbConnection m_connection;
        int m_crawlerId;

        public DataProvider(database.IDbConnection connection, int crawlerId)
        {
            m_connection = connection;
            m_crawlerId = crawlerId;
        }

        public List<string> getAllUrlsToLoadFromQueue()
        {
            m_connection.executeChangeQuery("UPDATE queue SET catched='1' WHERE crawler_id='" + m_crawlerId + "'");
            List<object>[] queryResult = m_connection.executeReadQuery("SELECT web_resource_id, path FROM queue WHERE crawler_id='" + m_crawlerId + "'");

            List<string> result = new List<string>();

            if (result == null || queryResult.Length == 0)
            {
                return result;
            }

            for (int i = 0; i < queryResult[0].Count && queryResult[0][i] != null; ++i)
            {
                string url = getWebResourceUrlById(int.Parse(queryResult[0][i].ToString()));

                if (url == null)
                {
                    continue;
                }

                result.Add(url + queryResult[1][i].ToString());
            }

            return result;
        }

        public void removeAllCatchedUrls()
        {
            m_connection.executeChangeQuery("DELETE FROM queue WHERE crawler_id='" + m_crawlerId + "' and catched='1'");
        }

        string getWebResourceUrlById(int id)
        {
            List<object>[] result = m_connection.executeReadQuery("SELECT url FROM web_resources WHERE id='" + id + "'");

            if (result.Length == 0)
            {
                return null;
            }

            return result[0][0].ToString();
        }
    }
}
