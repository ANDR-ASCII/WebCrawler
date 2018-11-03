using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace webcrawler.loader
{
    class Loader
    {
        public Loader()
        {
            _webDriver = new ChromeDriver(Directory.GetCurrentDirectory());
        }

        private IWebDriver _webDriver;
    }
}
