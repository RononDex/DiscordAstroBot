using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAstroBot.Mappers.Simbad;

namespace DiscordAstroBot.Objects.Simbad
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
        public static AstronomicalObjectInfo FromSimbadResult(SimbadResult result)
        {
            var obj = new AstronomicalObjectInfo();
            if (result.Sections.ContainsKey("Main_id"))
                obj.Name = result.Sections["Main_id"].Replace("\n", "").Replace("\r", "");
            if (result.Sections.ContainsKey("ObjectType"))
                obj.ObjectType = result.Sections["ObjectType"];
            if (result.Sections.ContainsKey("Coordinates"))
                obj.Coordinates = RADECCoords.FromSimbadString(result.Sections["Coordinates"]);

            if (result.Sections.ContainsKey("ProperMotion"))
                obj.ProperMotion = RADECCoords.FromSimbadString(result.Sections["ProperMotion"]);

            if (result.Sections.ContainsKey("Identifiers"))
                obj.AlsoKnownAs.AddRange(result.Sections["Identifiers"].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("\n", "").Replace("\r", "").Trim()));

            if (result.Sections.ContainsKey("Parallax"))
                obj.Parallax = ValueWithError.FromSimbadString(result.Sections["Parallax"]);

            if (result.Sections.ContainsKey("RadialVelocity"))
                obj.RadialVelocity = RadialVelocity.FromSimbadString(result.Sections["RadialVelocity"]);

            if (result.Sections.ContainsKey("OtherTypes"))
            {
                obj.SecondaryTypes.AddRange(result.Sections["OtherTypes"].Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
            }

            if (result.Sections.ContainsKey("Distances"))
            {
                var lines = result.Sections["Distances"].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("\n", "").Replace("\r", "")).ToList();
                lines.RemoveAt(0);
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    obj.DistanceMeasurements.Add(DistanceMeasurement.FromSimbadString(line));
                }
            }

            if (result.Sections.ContainsKey("Dimensions"))
                obj.AngularDimension = ObjectDimensions.FromSimbadString(result.Sections["Dimensions"]);

            if (result.Sections.ContainsKey("Fluxes"))
            {
                var lines = result.Sections["Fluxes"].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Replace("\n", "").Replace("\r", "")).ToList();
                foreach (var line in lines)
                {
                    var trimmed = line.Replace("\n", "").Replace("\r", "");

                    if (string.IsNullOrEmpty(trimmed))
                        continue;

                    obj.Magntiudes.Add(Magnitude.FromSimbadString(trimmed));
                }
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

        /// <summary>
        /// A list of synonyms for this object
        /// </summary>
        public List<string> AlsoKnownAs { get; set; } = new List<string>();

        /// <summary>
        /// The distance measurements for this object
        /// </summary>
        public List<DistanceMeasurement> DistanceMeasurements { get; set; } = new List<DistanceMeasurement>();

        /// <summary>
        /// THe known magnitudes for this object
        /// </summary>
        public List<Magnitude> Magntiudes { get; set; } = new List<Magnitude>();

        /// <summary>
        /// The angular dimension of the object if known
        /// </summary>
        public ObjectDimensions AngularDimension { get; set; }
    }

    /// <summary>
    /// Represents a distance measurement for an astronomical object
    /// </summary>
    public class DistanceMeasurement
    {
        public static DistanceMeasurement FromSimbadString(string result)
        {
            var segments = result.Split(new string[] { "|" }, StringSplitOptions.None);
            var dist = segments[1].Substring(0, 8);
            var unit = segments[1].Substring(10, 4);
            var errMinus = segments[2].Substring(0, 11);
            var errPlus = segments[2].Substring(10, 8);
            var method = segments[3];
            var reference = segments[4];

            return new DistanceMeasurement()
            {
                Distance = dist,
                ErrMinus = errMinus,
                ErrPlus = errPlus,
                Method = method,
                Reference = reference,
                Unit = unit
            };
        }

        /// <summary>
        /// Measured distance
        /// </summary>
        public string Distance { get; set; }

        /// <summary>
        /// Unit of the given measurement
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Error-
        /// </summary>
        public string ErrMinus { get; set; }

        /// <summary>
        /// Error+
        /// </summary>
        public string ErrPlus { get; set; }

        /// <summary>
        /// Measurement method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Reference
        /// </summary>
        public string Reference { get; set; }

        public override string ToString()
        {
            var method = "";
            if (!string.IsNullOrEmpty(this.Method))
                method = $"    Method: {Method}";

            var error = "";
            if (!string.IsNullOrEmpty(this.ErrMinus))
                error = $"    Error: {ErrPlus} {ErrMinus}";

            return $"{Distance} {Unit}{error}{method}";
        }
    }

    /// <summary>
    /// Describes the angular dimensions of an object (limited to mag25 dimension)
    /// </summary>
    public class ObjectDimensions
    {
        /// <summary>
        /// Size in X axis in arcminutes
        /// </summary>
        public float XSize { get; set; }

        /// <summary>
        /// Size in Y axis in arcminutes
        /// </summary>
        public float YSize { get; set; }

        /// <summary>
        /// The rotation of the object in degrees (0-180°)
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Gets a ObjectDimensions object from a serialized SIMBAD string
        /// </summary>
        /// <param name="simbadValue"></param>
        /// <returns></returns>
        public static ObjectDimensions FromSimbadString(string simbadValue)
        {
            var segments = simbadValue.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (segments == null || segments.Length < 3)
                return null;

            if (segments[0] == "~")
                return null;

            var result = new ObjectDimensions();
            result.XSize = Convert.ToSingle(segments[0]);
            result.YSize = Convert.ToSingle(segments[1]);

            if (!segments[2].StartsWith("~"))
                result.Rotation = Convert.ToSingle(segments[2]);

            return result;
        }
    }

    /// <summary>
    /// Represents the coordinates of a DSO
    /// </summary>
    public class RADECCoords
    {
        public static RADECCoords FromSimbadString(string result)
        {
            var dec = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("dec:")).Replace("DEC: ", "").Replace("\n", "").Replace("\r", "");
            var ra = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("ra:")).Replace("RA: ", "").Replace("\n", "").Replace("\r", "");

            if (string.IsNullOrEmpty(dec) || string.IsNullOrEmpty(ra) || dec.Contains("~") || ra.Contains("~"))
                return null;

            return new RADECCoords() { DEC = Convert.ToSingle(dec), RA = Convert.ToSingle(ra) };
        }

        /// <summary>
        /// Right-ascension angle
        /// </summary>
        public float RA { get; set; }

        /// <summary>
        /// Declenation angle
        /// </summary>
        public float DEC { get; set; }

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
    /// Respresents the magnitude of an object for a given wavelength (filter) 
    /// </summary>
    public class Magnitude
    {
        public static Magnitude FromSimbadString(string result)
        {
            var values = result.Split(new[] { "=" }, StringSplitOptions.None);
            var filter = values[0].Replace("\n", "").Replace("\r", "");
            var value = values[1].Replace("\n", "").Replace("\r", "");

            return new Magnitude() { Filter = filter, Value = value };
        }

        public string Filter { get; set; }

        public string FilterDesc
        {
            get
            {
                switch (this.Filter.ToUpper())
                {
                    case "U": return "   365nm -    66nm, Ultraviolet";
                    case "B": return "   445nm -    94nm, Visible / Blue";
                    case "V": return "   551nm -    88nm, Visible / Visual";
                    case "G": return " UNKNOWN - UNKNOWN, Visible / Green";
                    case "R": return "   658nm -   138nm, Visible / Red";
                    case "I": return "   806nm -   149nm, Near-Infrared";
                    case "Z": return "   900nm - UNKNOWN, Near-Infrared";
                    case "Y": return "  1020nm -   120nm, Near-Infrared";
                    case "J": return "  1220nm -   213nm, Near-Infrared";
                    case "H": return "  1630nm -   307nm, Near-Infrared";
                    case "K": return "  2190nm -   390nm, Near-Infrared";
                    case "L": return "  3450nm -   472nm, Near-Infrared";
                    case "M": return "  4750nm -   460nm, Mid-Infrared";
                    case "N": return " 10500nm -  2500nm, Mid-Infrared";
                    case "Q": return " 21000nm -  5800nm, Mid-Infrared";
                    default: return "UNKNOWN FILTER";
                }
            }
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{this.Filter}: {this.Value}    ({this.FilterDesc})";
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
            var err = result.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x => x.ToLower().StartsWith("error:")).Trim().Replace("\n", "").Replace("\r", "").Replace("Error: ", "");
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
