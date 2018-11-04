using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace webcrawler.loader
{
    class Loader
    {
        int m_crawlerId;
        OpenQA.Selenium.IWebDriver m_webDriver;
        database.IDbConnection m_connection;
        DataProvider m_dataProvider;

        public Loader(int crawlerId, database.IDbConnection connection)
        {
            m_crawlerId = crawlerId;

            initializeWebDriver();

            m_connection = connection;
            m_dataProvider = new DataProvider(m_connection, m_crawlerId);
        }

        public void start()
        {
            while (true)
            {
                checkQueue();
                Thread.Sleep(100);
            }
        }

        void checkQueue()
        {
            List<string> urls = m_dataProvider.getAllUrlsToLoadFromQueue();

            if (urls.Count == 0)
            {
                return;
            }

            loadUrls(urls);

            m_dataProvider.removeAllCatchedUrls();
        }

        void loadUrls(List<string> urls)
        {
            foreach (string url in urls)
            {
                loadUrl(url);
            }
        }

        void loadUrl(string url)
        {
            const string javaScriptWaitFunction =
                "function wait()" +
                "{" +
                "if(window.onLoad) { return; } else { setTimeout('wait()', 100); } " +
                "}" +
                "wait();";

            url = fixUrlProtocolIfNeeded(url);
            m_webDriver.Navigate().GoToUrl(url);

            OpenQA.Selenium.IJavaScriptExecutor javaScriptExecutor =
                m_webDriver as OpenQA.Selenium.IJavaScriptExecutor;

            javaScriptExecutor.ExecuteScript(javaScriptWaitFunction);

            System.Console.WriteLine(url + " loaded");
        }

        void initializeWebDriver()
        {
            OpenQA.Selenium.Chrome.ChromeOptions options = new OpenQA.Selenium.Chrome.ChromeOptions();
            options.AddArgument("--headless");

            m_webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Directory.GetCurrentDirectory(), options);
        }

        string fixUrlProtocolIfNeeded(string url)
        {
            if (!url.StartsWith("https://") && !url.StartsWith("http://"))
            {
                return new string("http://" + url);
            }

            return url;
        }
    }
}
