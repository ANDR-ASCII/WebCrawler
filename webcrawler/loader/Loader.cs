using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace webcrawler.loader
{
    class Loader
    {
        OpenQA.Selenium.IWebDriver _webDriver;
        List<string> _urls;
        object _locker;
        bool _stop;
        int _crawlerId;
        database.IDbConnection _connection;

        public Loader(int crawlerId, database.IDbConnection connection)
        {
            InitializeWebDriver();

            _urls = new List<string>();
            _locker = new object();

            _crawlerId = crawlerId;
            _connection = connection;
        }

        public void Start()
        {
            _stop = false;

            while (true)
            {
                lock (_locker)
                {
                    if (_stop)
                    {
                        break;
                    }
                }

                CheckQueue();
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            lock (_locker)
            {
                _stop = true;
            }
        }

        void CheckQueue()
        {
            List<string> urls = GetAllUrlsToLoadFromQueue();

            if (urls.Count == 0)
            {
                return;
            }

            LoadUrls(urls);

            _connection.ExecuteReadQuery("DELETE FROM queue WHERE crawler_id='" + _crawlerId + "' and catched='1'");
        }

        string GetWebResourceUrlById(int id)
        {
            List<object>[] result = _connection.ExecuteReadQuery("SELECT url FROM web_resources WHERE id='" + id + "'");

            if (result.Length == 0)
            {
                return null;
            }

            return result[0][0].ToString();
        }

        void LoadUrls(List<string> urls)
        {
            foreach (string url in urls)
            {
                LoadUrl(url);
            }
        }

        void LoadUrl(string url)
        {
            const string javaScriptWaitFunction =
                "function wait()" +
                "{" +
                "if(window.onLoad) { return; } else { setTimeout('wait()', 100); } " +
                "}" +
                "wait();";

            url = FixUrlProtocolIfNeeded(url);
            _webDriver.Navigate().GoToUrl(url);

            OpenQA.Selenium.IJavaScriptExecutor javaScriptExecutor =
                _webDriver as OpenQA.Selenium.IJavaScriptExecutor;

            javaScriptExecutor.ExecuteScript(javaScriptWaitFunction);

            System.Console.WriteLine(url + " loaded");
        }

        List<string> GetAllUrlsToLoadFromQueue()
        {
            _connection.ExecuteChangeQuery("UPDATE queue SET catched='1' WHERE crawler_id='" + _crawlerId + "'");
            List<object>[] queryResult = _connection.ExecuteReadQuery("SELECT web_resource_id, path FROM queue WHERE crawler_id='" + _crawlerId + "'");

            List<string> result = new List<string>();

            if (result == null || queryResult.Length == 0)
            {
                return result;
            }

            for (int i = 0; i < queryResult[0].Count && queryResult[0][i] != null; ++i)
            {
                string url = GetWebResourceUrlById(int.Parse(queryResult[0][i].ToString()));

                if (url == null)
                {
                    continue;
                }

                result.Add(url + queryResult[1][i].ToString());
            }

            return result;
        }

        void InitializeWebDriver()
        {
            OpenQA.Selenium.Chrome.ChromeOptions options = new OpenQA.Selenium.Chrome.ChromeOptions();
            options.AddArgument("--headless");

            _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Directory.GetCurrentDirectory(), options);
        }

        string FixUrlProtocolIfNeeded(string url)
        {
            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
            {
                return new string("http://" + url);
            }

            return url;
        }
    }
}
