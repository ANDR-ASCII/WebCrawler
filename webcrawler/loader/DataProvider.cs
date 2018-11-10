using System.Collections.Generic;

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
        database.IDbConnection m_connection;
        int m_crawlerId;

        public DataProvider(database.IDbConnection connection, int crawlerId)
        {
            m_connection = connection;
            m_crawlerId = crawlerId;
        }

        public void savePageData(PageData pageData)
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

            database.SqlCommandParameter sqlCommandParameter = new database.SqlCommandParameter();
            sqlCommandParameter.parameterName = "@datetime";
            sqlCommandParameter.type = database.ProcedureArgumentType.DateTime;
            sqlCommandParameter.value = pageData.dateTime;

            m_connection.executeNonQuery(insertDataQuery, sqlCommandParameter);
        }

        public List<ScheduledUrlData> getAllUrlsToLoadFromQueue()
        {
            m_connection.executeNonQuery("UPDATE queue SET catched='1' WHERE crawler_id='" + m_crawlerId + "'");
            List<object>[] queryResult = m_connection.executeReadQuery("SELECT web_resource_id, path FROM queue WHERE crawler_id='" + m_crawlerId + "'");

            List<ScheduledUrlData> result = new List<ScheduledUrlData>();

            if (result == null || queryResult.Length == 0)
            {
                return result;
            }

            for (int i = 0; i < queryResult[0].Count && queryResult[0][i] != null; ++i)
            {
                ScheduledUrlData scheduledUrlData;
                scheduledUrlData.webResourceId = int.Parse(queryResult[0][i].ToString());

                string url = getWebResourceUrlById(scheduledUrlData.webResourceId);

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

        public void removeAllCatchedUrls()
        {
            m_connection.executeNonQuery("DELETE FROM queue WHERE crawler_id='" + m_crawlerId + "' and catched='1'");
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
