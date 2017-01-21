using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects
{ 
    /// <summary>
    /// Represents information on an astronomical object
    /// </summary>
    public class AstronomicalObjectInfo
    {
        /// <summary>
        /// Creates an info object from a simbad query result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static AstronomicalObjectInfo FromSimbadResult(Simbad.SimbadResult result)
        {
            var obj = new AstronomicalObjectInfo();
            if (result.Sections.ContainsKey("Main_id"))
                obj.Name = result.Sections["Main_id"];
            if (result.Sections.ContainsKey("ObjectType"))
                obj.ObjectType = result.Sections["ObjectType"];
            if (result.Sections.ContainsKey("Coordinates"))
            {
                var dec = result.Sections["Coordinates"].Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault( x => x.ToLower().StartsWith("dec:"));
                var ra = result.Sections["Coordinates"].Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault( x => x.ToLower().StartsWith("ra:"));
                obj.Coordinates = new AstroSkyCoordinates()
                {
                    DEC = dec.ToLower().Replace("DEC: ", "").Replace("\n", "").Replace("\r", ""),
                    RA = ra.ToLower().Replace("RA: ", "").Replace("\n", "").Replace("\r", "")
                };
            }

            if (result.Sections.ContainsKey("OtherTypes"))
            {
                obj.SecondaryTypes.AddRange(result.Sections["OtherTypes"].Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
            }

            return obj;
        }

        /// <summary>
        /// The main name of the object (main identifier)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The main type of the object
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Coordinates (RA, DEC)
        /// </summary>
        public AstroSkyCoordinates Coordinates { get; set; }

        /// <summary>
        /// Secondary object types (also classified as...)
        /// </summary>
        public List<string> SecondaryTypes { get; set; } = new List<string>();
    }

    /// <summary>
    /// Represents the coordinates of a DSO
    /// </summary>
    public class AstroSkyCoordinates
    {
        /// <summary>
        /// Right-ascension angle
        /// </summary>
        public string RA { get; set; }

        /// <summary>
        /// Declenation angle
        /// </summary>
        public string DEC { get; set; }

        public override string ToString()
        {
            return string.Format("RA: {0}\tDEC: {1}", this.RA, this.DEC);
        }
    }
}
