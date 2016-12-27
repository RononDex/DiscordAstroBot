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

            var objectInfo = new AstronomicalObjectInfo();
            var simbadResolver = xml.Descendants("Resolver").First(x => x.FirstAttribute.Value.ToLower().Contains("simbad"));
            objectInfo.Aliases = simbadResolver.Descendants("alias").Select(x => x.Value).ToList();
            objectInfo.Name = simbadResolver.Element("oname").Value;
            objectInfo.DECDec = Convert.ToSingle(simbadResolver.Element("jdedeg").Value, CultureInfo.InvariantCulture);
            objectInfo.RADec = Convert.ToSingle(simbadResolver.Element("jradeg").Value, CultureInfo.InvariantCulture);
            objectInfo.PositionJ2000 = simbadResolver.Element("jpos").Value;

            foreach (var flux in simbadResolver.Elements("mag"))
            {
                objectInfo.Fluxes.Add(new Magnitude()
                {
                    Type = flux.Attribute("band").Value,
                    Value = Convert.ToSingle(flux.Element("v").Value, CultureInfo.InvariantCulture),
                    Error = Convert.ToSingle(flux.Element("e").Value, CultureInfo.InvariantCulture),
                    Reference = flux.Element("r").Value
                });
            }

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
    }

    public class Magnitude
    {
        public float Value { get; set; }

        public float Error { get; set; }

        public string Reference { get; set; }

        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("    Type: {1}    Value: {0}    Error: {2}\r\n    Reference: {3}", this.Value, this.Type, this.Error, this.Reference);
        }
    }
}
