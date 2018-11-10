using System;
using System.Collections.Generic;
using System.Text;

namespace webcrawler
{
    class Configuration
    {
        static string s_crawlerIdParamName = "crawler_id";
        static string s_dbParamName = "database";
        static string s_serverAddressParamName = "server";
        static string s_dbUserParamName = "db_user";
        static string s_dbUserPasswordParamName = "db_user_password";

        string m_databaseName;
        string m_server;
        string m_user;
        string m_password;
        int m_crawlerId;
        bool m_showHelp;
        NDesk.Options.OptionSet m_optionsSet;

        public Configuration(string[] args)
        {
            Dictionary<string, string> commandParams =
                new Dictionary<string, string>();

            m_optionsSet = new NDesk.Options.OptionSet()
            {
                { s_crawlerIdParamName + "=", "set crawler id.", v => commandParams.Add(s_crawlerIdParamName, v) },
                { "h|help", "show this message and exit.", v => m_showHelp = (v != null) },
                { s_dbParamName + "=", "sets the database name to connect.", v => commandParams.Add(s_crawlerIdParamName, v) },
                { s_serverAddressParamName + "=", "sets the database server address.", v => commandParams.Add(s_crawlerIdParamName, v) },
                { s_dbUserParamName + "=", "sets the database user name.", v => commandParams.Add(s_crawlerIdParamName, v) },
                { s_dbUserPasswordParamName + "=", "sets the database user password.", v => commandParams.Add(s_crawlerIdParamName, v) }
            };

            m_optionsSet.Parse(args);

            m_crawlerId = commandParams.ContainsKey(s_crawlerIdParamName) ? int.Parse(commandParams[s_crawlerIdParamName]) : 0;
            m_databaseName = commandParams.ContainsKey(s_dbParamName) ? commandParams[s_dbParamName] : "webcrawler";
            m_server = commandParams.ContainsKey(s_serverAddressParamName) ? commandParams[s_serverAddressParamName] : "127.0.0.1";
            m_user = commandParams.ContainsKey(s_dbUserParamName) ? commandParams[s_dbUserParamName] : "root";
            m_password = commandParams.ContainsKey(s_dbUserPasswordParamName) ? commandParams[s_dbUserPasswordParamName] : "root";
        }

        public string DatabaseName
        {
            get { return m_databaseName; }
        }
        public string Server
        {
            get { return m_server; }
        }
        public string User
        {
            get { return m_user; }
        }
        public string Password
        {
            get { return m_password; }
        }
        public int CrawlerId
        {
            get { return m_crawlerId; }
        }
        public bool ShowHelp
        {
            get { return m_showHelp; }
        }
        public NDesk.Options.OptionSet OptionsSet
        {
            get { return m_optionsSet; }
        }
    }
}
