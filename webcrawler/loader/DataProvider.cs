using System.Collections.Generic;
using webcrawler.database;

namespace webcrawler.loader
{
    struct PageData
    {
        public int webResourceId;
        public string path;
        public string htmlCode;
        public string serverResponse;
        public string protocol;
        public System.DateTime dateTime;
        public int statusCode;
    }

    struct ScheduledUrlData
    {
        public int webResourceId;
        public string host;
        public string path;
    }

    class DataProvider
    {
        private IDbConnection m_connection;
        private uint m_crawlerId;

        public DataProvider(IDbConnection connection, uint crawlerId)
        {
            m_connection = connection;
            m_crawlerId = crawlerId;
        }

        public void SavePageData(PageData pageData)
        {
            string insertDataQuery = "INSERT INTO data (web_resource_id, path, html_code, server_response, protocol, date_time, status_code)" +
                "VALUES " +
                "('" + pageData.webResourceId +"', " +
                "'" + pageData.path + "', " +
                "'" + pageData.htmlCode + "', " +
                "'" + pageData.serverResponse + "', " +
                "'" + pageData.protocol + "', " +
                "@datetime, " +
                "'" + pageData.statusCode + "')";

            SqlCommandParameter sqlCommandParameter = new SqlCommandParameter();
            sqlCommandParameter.parameterName = "@datetime";
            sqlCommandParameter.type = ProcedureArgumentType.DateTime;
            sqlCommandParameter.value = pageData.dateTime;

            m_connection.ExecuteNonQuery(insertDataQuery, sqlCommandParameter);
        }

        public List<ScheduledUrlData> GetAllUrlsToLoadFromQueue()
        {
            m_connection.ExecuteNonQuery("UPDATE queue SET catched='1' WHERE crawler_id='" + m_crawlerId + "'");
            List<object>[] queryResult = m_connection.ExecuteReadQuery("SELECT web_resource_id, path FROM queue WHERE crawler_id='" + m_crawlerId + "'");

            List<ScheduledUrlData> result = new List<ScheduledUrlData>();

            if (result == null || queryResult.Length == 0)
            {
                return result;
            }

            for (int i = 0; i < queryResult[0].Count && queryResult[0][i] != null; ++i)
            {
                ScheduledUrlData scheduledUrlData;
                scheduledUrlData.webResourceId = int.Parse(queryResult[0][i].ToString());

                string url = GetWebResourceUrlById(scheduledUrlData.webResourceId);

                if (url == null)
                {
                    continue;
                }

                scheduledUrlData.host = url;
                scheduledUrlData.path = queryResult[1][i].ToString();

                result.Add(scheduledUrlData);
            }

            return result;
        }

        public void RemoveAllCatchedUrls()
        {
            m_connection.ExecuteNonQuery("DELETE FROM queue WHERE crawler_id='" + m_crawlerId + "' and catched='1'");
        }

        string GetWebResourceUrlById(int id)
        {
            List<object>[] result = m_connection.ExecuteReadQuery("SELECT url FROM web_resources WHERE id='" + id + "'");

            if (result.Length == 0)
            {
                return null;
            }

            return result[0][0].ToString();
        }
    }
}
