using System;
using System.Collections.Generic;

namespace webcrawler
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            try
            {
                Configuration configuration = new Configuration(args);

                if (configuration.ShowHelp)
                {
                    ShowHelp(configuration.OptionsSet);
                    return;
                }

                database.IDbConnection connection =
                    database.ConnectionFactory.CreateConnection(database.DatabaseType.DatabaseMySql);

                connection.setDatabaseName(configuration.DatabaseName);
                connection.setServerAddress(configuration.Server);
                connection.setUserName(configuration.User);
                connection.setUserPassword(configuration.Password);
                connection.open();

                if (!RegistrateCrawlerId(connection, configuration.CrawlerId))
                {
                    Console.WriteLine("The crawler id {0} is already registered.\n" +
                        "Please set another id to ensure correct start.", configuration.CrawlerId);

                    return;
                }

                loader.Loader loader = new loader.Loader(configuration.CrawlerId, connection);

                System.Threading.Thread loaderThread = new System.Threading.Thread(loader.start);
                loaderThread.Start();
                loaderThread.Join();

                UnregistrateCrawlerId(connection, configuration.CrawlerId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        static void ShowHelp(NDesk.Options.OptionSet p)
        {
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        static bool RegistrateCrawlerId(database.IDbConnection connection, int id)
        {
            string query;

            using (System.IO.StreamReader stream = new System.IO.StreamReader("registrate_crawler_id.sql"))
            {
                query = stream.ReadToEnd();
            }

            if (query == null)
            {
                return false;
            }

            Dictionary<string, (database.ProcedureArgumentType, bool)> procedureArguments =
                new Dictionary<string, (database.ProcedureArgumentType, bool)>();

            procedureArguments["id"] = (database.ProcedureArgumentType.Int32, false);
            connection.executeStoredProcedure("registrate_crawler_id", query, procedureArguments);

            return true;
        }
        static void UnregistrateCrawlerId(database.IDbConnection connection, int id)
        {
            connection.executeNonQuery("DELETE FROM crawler_id WHERE id='" + id + "'");
        }
    }
}
