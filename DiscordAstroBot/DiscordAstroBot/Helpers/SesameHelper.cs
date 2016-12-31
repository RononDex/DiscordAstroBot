using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiscordAstroBot.Helpers
{
    /// <summary>
    /// Helper class to help query the Sesame webservice
    /// </summary>
    public static class SesameHelper
    {
        public static AstronomicalObjectInfo ResolveWithSesame(string name)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Log<DiscordAstroBot>.InfoFormat("Requesting astronomical info for {0}", name);

            var webRequest = WebRequest.Create(string.Format("http://cdsweb.u-strasbg.fr/cgi-bin/nph-sesame/-oIfx?{0}", WebUtility.UrlEncode(name)));
            var response = (HttpWebResponse)webRequest.GetResponse();

            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            // Parse XML answer
            var xml = XDocument.Parse(text);

            // Check if a result was found
            var objectInfo = new AstronomicalObjectInfo();
            var simbadResolver = xml.Descendants("Resolver").FirstOrDefault(x => x.FirstAttribute.Value.ToLower().Contains("simbad"));
            if (simbadResolver == null) return null;

            // Readout information from simbad
            objectInfo.Aliases = simbadResolver.Descendants("alias").Select(x => x.Value).ToList();
            objectInfo.Name = simbadResolver.Element("oname").Value;
            objectInfo.DECDec = Convert.ToSingle(simbadResolver.Element("jdedeg").Value, CultureInfo.InvariantCulture);
            objectInfo.RADec = Convert.ToSingle(simbadResolver.Element("jradeg").Value, CultureInfo.InvariantCulture);
            objectInfo.PositionJ2000 = simbadResolver.Element("jpos").Value;

            foreach (var flux in simbadResolver.Elements("mag"))
            {
                objectInfo.Fluxes.Add(new Magnitude()
                {
                    Filter = Magnitude.GetFilterFriendlyName(flux.Attribute("band").Value),
                    Value = Convert.ToSingle(flux.Element("v").Value, CultureInfo.InvariantCulture),
                    Error = Convert.ToSingle(flux.Element("e").Value, CultureInfo.InvariantCulture),
                    Reference = flux.Element("r").Value
                });
            }

            var velocityTag = simbadResolver.Element("Vel");
            objectInfo.Velocity = new Velocity()
            {
                VelocityError =Convert.ToSingle(velocityTag.Element("e").Value),
                Reference = velocityTag.Element("r").Value,
                RelVelocity = Convert.ToSingle(velocityTag.Element("v").Value)
            };

            return objectInfo;
        }
    }

    public class AstronomicalObjectInfo
    {
        public string PositionJ2000 { get; set; }

        public float RADec { get; set; }

        public float DECDec { get; set; }

        public string Name { get; set; }

        public List<string> Aliases { get; set; } = new List<string>();

        public List<Magnitude> Fluxes { get; set; } = new List<Magnitude>();

        public Velocity Velocity { get; set; }
    }

    public class Magnitude
    {
        public float Value { get; set; }

        public float Error { get; set; }

        public string Reference { get; set; }

        public string Filter { get; set; }

        public static string GetFilterFriendlyName(string filter)
        {
            switch (filter.ToUpper())
            {
                case "U": return "Ultra-Violet";
                case "B": return "Blue";
                case "V": return "Visible";
                case "G": return "Green";
                case "R": return "Red";
                case "I": return "Infra-Red";
                default: return filter;
            }
        }

        public override string ToString()
        {
            return string.Format("    Filter: {1}    Value: {0} ± {2}\r\n    Reference: {3}", this.Value, this.Filter, this.Error, this.Reference);
        }
    }

    public class Velocity
    {
        const float LIGHTSPEED = 2.998e5F;

        /// <summary>
        /// Relative velocity in km/s
        /// </summary>
        public float RelVelocity { get; set; }

        /// <summary>
        /// The error on its velocity measurement
        /// </summary>
        public float VelocityError { get; set; }

        /// <summary>
        /// Reference to the measurement
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Get the corresponding redshift to the velocity
        /// </summary>
        public float Redshift
        {
            get
            {
                return Convert.ToSingle(Math.Sqrt((1 + (this.RelVelocity / LIGHTSPEED)) / (1 - (this.RelVelocity / LIGHTSPEED)))) - 1;
            }
        }

        public override string ToString()
        {
            return string.Format("    Value: ({0} ± {1}) km/s    Redshift: {2}\r\n    Reference: {3}", this.RelVelocity, this.VelocityError, this.Redshift, this.Reference);
        }
    }
}
