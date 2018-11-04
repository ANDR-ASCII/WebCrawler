using System;
using System.Collections.Generic;

namespace webcrawler
{
    class EntryPoint
    {
        static string s_crawlerIdParamName = "crawler_id";
        static string s_dbParamName = "database";
        static string s_serverAddressParamName = "server";
        static string s_dbUserParamName = "db_user";
        static string s_dbUserPasswordParamName = "db_user_password";

        static void Main(string[] args)
        {
            bool showHelp = false;

            Dictionary<string, string> commandParams =
                new Dictionary<string, string>();

            var optionSet = new NDesk.Options.OptionSet()
            {
                { s_crawlerIdParamName + "=", "set crawler id.", v => commandParams.Add(s_crawlerIdParamName, v) },
                { "h|help", "show this message and exit.", v => showHelp = (v != null) },
                { s_dbParamName + "=", "sets the database name to connect.", v => commandParams.Add(s_crawlerIdParamName, v) },
                { s_serverAddressParamName + "=", "sets the database server address.", v => commandParams.Add(s_crawlerIdParamName, v) },
                { s_dbUserParamName + "=", "sets the database user name.", v => commandParams.Add(s_crawlerIdParamName, v) },
                { s_dbUserPasswordParamName + "=", "sets the database user password.", v => commandParams.Add(s_crawlerIdParamName, v) }
            };

            try
            {
                optionSet.Parse(args);
            }
            catch (NDesk.Options.OptionException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (showHelp)
            {
                ShowHelp(optionSet);
                return;
            }

            int crawlerId = commandParams.ContainsKey(s_crawlerIdParamName) ? int.Parse(commandParams[s_crawlerIdParamName]) : 0;
            string databaseName = commandParams.ContainsKey(s_dbParamName) ? commandParams[s_dbParamName] : "webcrawler";
            string server = commandParams.ContainsKey(s_serverAddressParamName) ? commandParams[s_serverAddressParamName] : "127.0.0.1";
            string user = commandParams.ContainsKey(s_dbUserParamName) ? commandParams[s_dbUserParamName] : "root";
            string password = commandParams.ContainsKey(s_dbUserPasswordParamName) ? commandParams[s_dbUserPasswordParamName] : "root";

            database.IDbConnection connection =
                database.ConnectionFactory.CreateConnection(database.DatabaseType.DatabaseMySql);

            connection.setDatabaseName(databaseName);
            connection.setServerAddress(server);
            connection.setUserName(user);
            connection.setUserPassword(password);
            connection.open();

            if (IsCrawlerIdAlreadyRegistered(connection, crawlerId) &&
                RegistrateCrawlerId(connection, crawlerId))
            {
                Console.WriteLine("The crawler id {0} is already registered.\n" +
                    "Please set another id to ensure correct start.", crawlerId);

                return;
            }

            loader.Loader loader = new loader.Loader(crawlerId, connection);

            System.Threading.Thread loaderThread = new System.Threading.Thread(loader.start);
            loaderThread.Start();
            loaderThread.Join();

            UnregistrateCrawlerId(connection, crawlerId);
        }

        static void ShowHelp(NDesk.Options.OptionSet p)
        {
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        static bool IsCrawlerIdAlreadyRegistered(database.IDbConnection connection, int id)
        {
            List<object>[] result = connection.executeReadQuery("SELECT id FROM crawler_id WHERE id='" + id + "'");
            return result.Length == 1 && result[0].Count != 0;
        }
        static bool RegistrateCrawlerId(database.IDbConnection connection, int id)
        {
            connection.executeChangeQuery("IF (SELECT id FROM crawler_id WHERE id='" + id + "') = NULL " +
                "INSERT INTO crawler_id (id) VALUES('" + id + "') ");

            return true;
        }
        static void UnregistrateCrawlerId(database.IDbConnection connection, int id)
        {
            connection.executeChangeQuery("DELETE FROM crawler_id WHERE id='" + id + "'");
        }
    }
}
