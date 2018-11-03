using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using MySql.Data.MySqlClient;

namespace webcrawler
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            loader.CommandWatcher watcher = new loader.CommandWatcher(0);
            watcher.StartWatching();

            /*try
            {
                IWebDriver driver = new ChromeDriver(Directory.GetCurrentDirectory());

                //Notice navigation is slightly different than the Java version
                //This is because 'get' is a keyword in C#
                driver.Navigate().GoToUrl("https://google.com");

                // Find the text input element by its name
                IWebElement query = driver.FindElement(By.Name("q"));

                // Enter something to search for
                query.SendKeys("Cheese");

                // Now submit the form. WebDriver will find the form for us from the element
                query.Submit();

                // Google's search is rendered dynamically with JavaScript.
                // Wait for the page to load, timeout after 10 seconds
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.Title.StartsWith("cheese", StringComparison.OrdinalIgnoreCase));

                // Should see: "Cheese - Google Search" (for an English locale)
                Console.WriteLine("Page title is: " + driver.Title);

                using (MySqlConnection connection = new MySqlConnection("server=127.0.0.1;uid=root;pwd=root;database=webcrawler"))
                {
                    connection.Open();
                }
            }
            catch(System.Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.ReadKey();
            }*/
        }
    }
}
