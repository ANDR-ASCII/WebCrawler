using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json.Linq;

namespace webcrawler.loader
{
    class Loader
    {
        int m_crawlerId;
        IWebDriver m_webDriver;
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
            List<ScheduledUrlData> urls = m_dataProvider.getAllUrlsToLoadFromQueue();

            if (urls.Count == 0)
            {
                return;
            }

            loadUrls(urls);

            m_dataProvider.removeAllCatchedUrls();
        }

        void loadUrls(List<ScheduledUrlData> scheduledUrlDataList)
        {
            foreach (ScheduledUrlData scheduledUrlData in scheduledUrlDataList)
            {
                loadUrl(scheduledUrlData);
            }
        }

        void loadUrl(ScheduledUrlData scheduledUrlData)
        {
            const string javaScriptWaitFunction =
                "function wait()" +
                "{" +
                "if(window.onLoad) { return; } else { setTimeout('wait()', 100); } " +
                "}" +
                "wait();";

            string targetResource = Helpers.fixUrlProtocolIfNeeded(scheduledUrlData.host + scheduledUrlData.path);
            m_webDriver.Navigate().GoToUrl(targetResource);

            IJavaScriptExecutor javaScriptExecutor = m_webDriver as IJavaScriptExecutor;
            javaScriptExecutor.ExecuteScript(javaScriptWaitFunction);

            Console.WriteLine(targetResource + " loaded");

            m_dataProvider.savePageData(preparePageData(scheduledUrlData));
        }

        void initializeWebDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.SetLoggingPreference("performance", LogLevel.All);

            m_webDriver = new ChromeDriver(Directory.GetCurrentDirectory(), options);
        }

        PageData preparePageData(ScheduledUrlData scheduledUrlData)
        {
            PageData pageData = new PageData();
            pageData.webResourceId = scheduledUrlData.webResourceId;
            pageData.path = scheduledUrlData.path;
            pageData.htmlCode = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(m_webDriver.PageSource));

            ReadOnlyCollection<LogEntry> performanceLogs =
                m_webDriver.Manage().Logs.GetLog("performance");

            foreach (LogEntry logEntry in performanceLogs)
            {
                JObject jsonObject;

                try
                {
                    jsonObject = JObject.Parse(logEntry.Message);

                    string methodString = (string)jsonObject["message"]["method"];

                    if (!methodString.StartsWith("Network.responseReceived"))
                    {
                        continue;
                    }

                    JObject paramsObject = (JObject)jsonObject["message"]["params"];
                    JObject response = (JObject)paramsObject["response"];

                    string urlString = (string)response["url"];
                    int remotePort = extractRemotePort(response);

                    Uri loadedUrl = new Uri(urlString);

                    UriBuilder expectedUrlBuilder = new UriBuilder(loadedUrl.Scheme,
                        scheduledUrlData.host,
                        remotePort,
                        scheduledUrlData.path);

                    if (!Helpers.compareUrls(loadedUrl, expectedUrlBuilder.Uri))
                    {
                        continue;
                    }

                    // TODO: here we must not to continue loop execution by throwing an exception
                    // otherwise we lose a valid target URL information

                    JObject headers = (JObject)response["headers"];

                    pageData.protocol = loadedUrl.Scheme;
                    pageData.statusCode = (int)response["status"];
                    pageData.dateTime = DateTime.Now;
                    pageData.serverResponse = headers.ToString();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }

            return pageData;
        }

        int extractRemotePort(JObject json)
        {
            JToken token;

            if (json.TryGetValue("remotePort", out token))
            {
                return (int)token;
            }

            return 80;
        }
    }
}
