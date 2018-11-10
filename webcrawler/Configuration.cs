using System.Collections.Generic;

namespace webcrawler
{
    class Configuration
    {
        static string s_dbParamName = "database";
        static string s_serverAddressParamName = "server";
        static string s_dbUserParamName = "db_user";
        static string s_dbUserPasswordParamName = "db_user_password";

        string m_databaseName;
        string m_server;
        string m_user;
        string m_password;
        bool m_showHelp;
        NDesk.Options.OptionSet m_optionsSet;

        public Configuration(string[] args)
        {
            Dictionary<string, string> commandParams =
                new Dictionary<string, string>();

            m_optionsSet = new NDesk.Options.OptionSet()
            {
                { "h|help", "show this message and exit.", v => m_showHelp = (v != null) },
                { s_dbParamName + "=", "sets the database name to connect.", v => commandParams.Add(s_dbParamName, v) },
                { s_serverAddressParamName + "=", "sets the database server address.", v => commandParams.Add(s_serverAddressParamName, v) },
                { s_dbUserParamName + "=", "sets the database user name.", v => commandParams.Add(s_dbUserParamName, v) },
                { s_dbUserPasswordParamName + "=", "sets the database user password.", v => commandParams.Add(s_dbUserPasswordParamName, v) }
            };

            m_optionsSet.Parse(args);

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
