using System;
using System.Collections.Generic;
using System.Threading;
using webcrawler.database;
using webcrawler.loader;

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

                CrawlerRegistrator registrator = new CrawlerRegistrator(DatabaseType.DatabaseMySql,
                    configuration.DatabaseName,
                    configuration.Server,
                    configuration.User,
                    configuration.Password);

                Loader loader = new Loader(registrator.CrawlerId, registrator.DbConnection);

                Thread loaderThread = new Thread(loader.Start);
                loaderThread.Start();
                loaderThread.Join();
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
    }
}
