using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace webcrawler.loader
{
    class CommandWatcher
    {
        const int c_watchPauseTime = 5000;
        database.IDbConnection _connection;
        int _crawlerId;
        List<Loader> _loaders;


        public CommandWatcher(int crawlerId, int loaderCount)
        {
            _connection = ConnectionFactory.CreateConnection(DatabaseType.DatabaseMySql);
            _connection.SetDatabaseName("webcrawler");
            _connection.SetServerAddress("127.0.0.1");
            _connection.SetUserName("root");
            _connection.SetUserPassword("root");
            _connection.Open();

            _crawlerId = crawlerId;
            _loaders = new List<Loader>();

            for (int i = 0; i < loaderCount; ++i)
            {
                _loaders.Add(new Loader(_crawlerId, _connection));
                Parallel.Invoke(_loaders[i].Start);
            }
        }

        public void StartWatching()
        {
            while (true)
            {
                Thread.Sleep(c_watchPauseTime);
            }
        }
    }
}
