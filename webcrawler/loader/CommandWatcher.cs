using System.Data;

namespace webcrawler.loader
{
    class CommandWatcher
    {
        public delegate void SettingsChangedCallback(DataTable datatable);

        public CommandWatcher()
        {
            _connection = ConnectionFactory.CreateConnection(DatabaseType.DatabaseMySql);
        }

        public void AddSettingsChangedCallback(SettingsChangedCallback callback)
        {
            _settingsChangedEvent += callback;
        }
        public void RemoveSettingsChangedCallbacks(SettingsChangedCallback callback)
        {
            _settingsChangedEvent -= callback;
        }

        private database.IDbConnection _connection;
        private event SettingsChangedCallback _settingsChangedEvent;
    }
}
