using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Helpers
{
    /// <summary>
    /// Helper class to get weather forcasts
    /// </summary>
    public static class WeatherHelper
    {
        public static WeatherForcast GetWeatherForcast(DateTime date, GeoLocation location)
        {
            Log<DiscordAstroBot>.InfoFormat("Requesting Weather for {0}", location.LocationName);

            // Opens the PhandomJS browser (hidden)
            using (var driver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver())
            {
                driver.Manage().Window.Size = new System.Drawing.Size(1500, 1080);

                // Navigate to clearoutside
                driver.Navigate().GoToUrl(string.Format("https://clearoutside.com/forecast/{0}/{1}", location.Lat, location.Long));

                // Wait until page is fully loaded
                IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
                wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Take a screenshoot
                Screenshot ss = driver.GetScreenshot();
                string screenshot = ss.AsBase64EncodedString;
                byte[] screenshotAsByteArray = ss.AsByteArray;

                return new WeatherForcast()
                {
                    Screenshot = screenshotAsByteArray
                };
            }
        }
    }

    public class WeatherForcast
    {
        public byte[] Screenshot { get; set; }
    }
}
