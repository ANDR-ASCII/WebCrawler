using System.Data;
using System.Threading;
using System.Collections.Generic;

namespace webcrawler.loader
{
    class CommandWatcher
    {
        public delegate void SettingsChangedCallback(DataTable datatable);

        public CommandWatcher(int crawlerGroupId)
        {
            _connection = ConnectionFactory.CreateConnection(DatabaseType.DatabaseMySql);
            _connection.SetDatabaseName("webcrawler");
            _connection.SetServerAddress("127.0.0.1");
            _connection.SetUserName("root");
            _connection.SetUserPassword("root");
            _connection.Open();

            _crawlerGroupId = crawlerGroupId;
            _loader = new Loader();
        }

        public void StartWatching()
        {
            while(true)
            {
                CheckQueue();

                Thread.Sleep(c_watchPauseTime);
            }
        }

        public void AddSettingsChangedCallback(SettingsChangedCallback callback)
        {
            _settingsChangedEvent += callback;
        }
        public void RemoveSettingsChangedCallbacks(SettingsChangedCallback callback)
        {
            _settingsChangedEvent -= callback;
        }

        private void CheckQueue()
        {
            List<object>[] result = _connection.ExecuteReadQuery("SELECT web_resource_id, path FROM queue WHERE crawler_group_id='" + _crawlerGroupId + "'");

            // 0 - web_resource_id
            // 1 - path

            if (result == null || result.Length == 0)
            {
                return;
            }

            for (int i = 0; i < result[0].Count && result[i] != null; ++i)
            {
                string url = GetWebResourceUrlById(int.Parse(result[0][i].ToString()));
                _loader.ScheduleUrl(url + result[1][i].ToString());
            }

            _loader.ExtractAndLoadUrl();
        }

        private string GetWebResourceUrlById(int id)
        {
            List<object>[] result = _connection.ExecuteReadQuery("SELECT url FROM web_resources WHERE id='" + id + "'");

            if (result.Length == 0)
            {
                return new string("");
            }

            return result[0][0].ToString();
        }

        //==========================================================================================================

        const int c_watchPauseTime = 5000;

        private database.IDbConnection _connection;
        private event SettingsChangedCallback _settingsChangedEvent;
        private int _crawlerGroupId;

        Loader _loader;
    }
}
