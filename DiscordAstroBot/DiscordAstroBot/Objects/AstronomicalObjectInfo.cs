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
                obj.Coordinates = RADECCoords.FromSimbadString(result.Sections["Coordinates"]);
            
            if (result.Sections.ContainsKey("ProperMotion"))            
                obj.Coordinates = RADECCoords.FromSimbadString(result.Sections["ProperMotion"]);
            

            if (result.Sections.ContainsKey("Parallax"))
                obj.Parallax = ValueWithError.FromSimbadString(result.Sections["Parallax"]);

            if (result.Sections.ContainsKey("RadialVelocity"))
                obj.RadialVelocity = RadialVelocity.FromSimbadString(result.Sections["RadialVelocity"]);

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
        public RADECCoords Coordinates { get; set; }

        /// <summary>
        /// Secondary object types (also classified as...)
        /// </summary>
        public List<string> SecondaryTypes { get; set; } = new List<string>();

        /// <summary>
        /// The proper motion of the object
        /// </summary>
        public RADECCoords ProperMotion { get; set; }

        /// <summary>
        /// Measured Parallax
        /// </summary>
        public ValueWithError Parallax { get; set; }

        public RadialVelocity RadialVelocity { get; set; }
    }

    /// <summary>
    /// Represents the coordinates of a DSO
    /// </summary>
    public class RADECCoords
    {
        public static RADECCoords FromSimbadString(string result)
        {
            var dec = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("dec:")).Replace("dec: ", "").Replace("\n", "").Replace("\r", "");
            var ra = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("ra:")).Replace("ra: ", "").Replace("\n", "").Replace("\r", "");

            return new RADECCoords() { DEC = dec, RA = ra };
        }

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
            return string.Format("RA: {0}\r\nDEC: {1}", this.RA, this.DEC);
        }
    }

    /// <summary>
    /// Represents a value that has an error
    /// </summary>
    public class ValueWithError
    {
        public static ValueWithError FromSimbadString(string result)
        {
            var value = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("value:")).Trim().Replace("\n", "").Replace("\r", "").Replace("value: ", "");
            var err = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("error:")).Trim().Replace("\n", "").Replace("\r", "").Replace("error: ", "");

            return new ValueWithError() { Value = value, Error = err };
        }

        public string Value { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ± {1}", this.Value, this.Error);
        }
    }

    /// <summary>
    /// Represents a Radial velocity data set from a space object
    /// </summary>
    public class RadialVelocity
    {
        public static RadialVelocity FromSimbadString(string result)
        {
            var z = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("z:")).Trim().Replace("\n", "").Replace("\r", "").Replace("z: ", "");
            var err = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("error:")).Trim().Replace("\n", "").Replace("\r", "").Replace("error: ", "");
            var v = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("v:")).Trim().Replace("\n", "").Replace("\r", "").Replace("v: ", "");

            return new RadialVelocity() { Redshift = z, Error = err, Velocity = v };
        }

        public string Redshift { get; set; }

        public string Error { get; set; }

        public string Velocity { get; set; }

        public override string ToString()
        {
            return string.Format("Z (redshift): {0}\r\nV: {1} km/s\r\nError: {2}", this.Redshift, this.Velocity, this.Error);
        }
    }
}
