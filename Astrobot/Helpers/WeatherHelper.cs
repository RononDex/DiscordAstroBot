using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Helpers
{
    /// <summary>
    /// Helper class to get weather forcasts
    /// </summary>
    public static class WeatherHelper
    {
        /// <summary>
        /// Gets a forecast from Clearoutside.com
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static WeatherForcast GetWeatherForcastClearOutside(GeoLocation location)
        {
            Log<DiscordAstroBot>.InfoFormat("Requesting Weather for {0}", location.LocationName);

            // Opens the PhandomJS browser (hidden)
            using (var driver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver())
            {
                driver.Manage().Window.Size = new System.Drawing.Size(1500, 1080);

                // Navigate to clearoutside
                driver.Navigate().GoToUrl($"https://clearoutside.com/forecast/{location.Lat}/{location.Long}");

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

        /// <summary>
        /// Gets a weather forcast from MeteoBlue.com
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static WeatherForcast GetWeatherForcastMeteoBlue(GeoLocation location)
        {
            Log<DiscordAstroBot>.InfoFormat("Requesting Weather for {0}", location.LocationName);

            // Opens the PhandomJS browser (hidden)
            using (var driver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver())
            {
                driver.Manage().Window.Size = new System.Drawing.Size(1500, 2560);

                var lat = (location.Lat >= 0) ? $"{location.Lat}N" : $"{Math.Abs(location.Lat)}S";
                var lng = (location.Long >= 0) ? $"{location.Long}E" : $"{Math.Abs(location.Long)}W";

                // Navigate to clearoutside
                driver.Navigate().GoToUrl($"https://www.meteoblue.com/en/weather/forecast/seeing/{lat}{lng}");

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

        public static MemoryStream GetWeatherForecastImgUrl(GeoLocation location)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var url = string.Format("https://clearoutside.com/forecast_image_large/{0:0.00}/{1:0.00}/forecast.png", location.Lat, location.Long);
            var wc = new WebClient();
            return new MemoryStream(wc.DownloadData(url));
        }
    }

    public class WeatherForcast
    {
        public byte[] Screenshot { get; set; }
    }
}
