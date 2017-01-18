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
                obj.Coordinates = result.Sections["Coordinates"];


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
        public string Coordinates { get; set; }
    }
}
