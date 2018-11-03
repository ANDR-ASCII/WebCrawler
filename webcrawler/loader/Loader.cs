using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Threading;

namespace webcrawler.loader
{
    class Loader
    {
        public Loader()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");

            _webDriver = new ChromeDriver(Directory.GetCurrentDirectory(), options);

            _urls = new List<string>();
            _mutex = new Mutex();
        }

        public void ScheduleUrl(string url)
        {
            Console.WriteLine("Url " + url + " was scheduled to load");

            _mutex.WaitOne();
            _urls.Add(url);
            _mutex.ReleaseMutex();
        }

        public void ExtractAndLoadUrl()
        {
            _mutex.WaitOne();

            if (_urls.Count == 0)
            {
                return;
            }

            string url = _urls[0];
            _urls.Remove(url);

            LoadUrl(url);

            _mutex.ReleaseMutex();
        }

        private void LoadUrl(string url)
        {
            string javaScriptWaitFunction =
                "function wait()" +
                "{" +
                "if(window.onLoad) { return; } else { setTimeout('wait()', 100); } " +
                "}" +
                "wait();";


            _webDriver.Navigate().GoToUrl("https://" + url);

             IJavaScriptExecutor javaScriptExecutor = _webDriver as IJavaScriptExecutor;
             javaScriptExecutor.ExecuteScript(javaScriptWaitFunction);

            IWebElement webElement = _webDriver.FindElement(By.XPath("//*[@id=\"home\"]/div/div/div[1]/div[2]/h1"));

            Console.WriteLine(webElement.Text);
        }

        private IWebDriver _webDriver;
        private List<string> _urls;
        private Mutex _mutex;
    }
}
