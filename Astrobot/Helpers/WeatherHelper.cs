using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DiscordAstroBot.Utilities;

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
                driver.Navigate().GoToUrl($"https://clearoutside.com/forecast/{location.Lat}/{location.Long}?view=midnight");

                // Wait until page is fully loaded
                IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
                wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Take a screenshoot
                Screenshot ss                = driver.GetScreenshot();
                string screenshot            = ss.AsBase64EncodedString;
                byte[] screenshotAsByteArray = ss.AsByteArray;

                // Crop the iamge
                using (var stream = new MemoryStream(screenshotAsByteArray))
                {
                    var image = new Bitmap(stream);

                    // Set bounds for the crop
                    var croppedImage = ImageUtility.CropImage(image, 165, 405, 1170, 1650);

                    // Convert image back to byte array
                    var stream2 = new MemoryStream();
                    croppedImage.Save(stream2, ImageFormat.Jpeg);
                    stream2.Position = 0;
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = stream2.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        screenshotAsByteArray = ms.ToArray();
                    }
                }

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

                // Crop the iamge
                using (var stream = new MemoryStream(screenshotAsByteArray))
                {
                    var image = new Bitmap(stream);

                    // Set bounds for the crop
                    var croppedImage = ImageUtility.CropImage(image, 295, 475, 1085, 2085);

                    // Convert image back to byte array
                    var stream2 = new MemoryStream();
                    croppedImage.Save(stream2, ImageFormat.Jpeg);
                    stream2.Position = 0;
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = stream2.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        screenshotAsByteArray = ms.ToArray();
                    }
                }

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
