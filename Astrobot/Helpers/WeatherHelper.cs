﻿using OpenQA.Selenium;
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
                Screenshot ss                = driver.GetScreenshot();
                string screenshot            = ss.AsBase64EncodedString;
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
                driver.Manage().Window.Size = new System.Drawing.Size(1500, 2000);

                var lat = $"{location.Lat}N";
                var lng = $"{location.Long}E";

                // Navigate to clearoutside
                driver.Navigate().GoToUrl($"https://www.meteoblue.com/en/weather/forecast/seeing/{lat}{lng}");

                // Wait until page is fully loaded
                IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
                wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Take a screenshoot
                Screenshot ss                = driver.GetScreenshot();
                string screenshot            = ss.AsBase64EncodedString;
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
