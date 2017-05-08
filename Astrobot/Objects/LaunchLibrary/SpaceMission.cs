using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Objects.LaunchLibrary
{
    /// <summary>
    /// Represents a space mission
    /// </summary>
    public class SpaceMission
    {
        /// <summary>
        /// The description of the mission
        /// </summary>
        public string Description { get; set;}

        /// <summary>
        /// A unique ID for the mission
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the mission (short)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the mission
        /// </summary>
        public string TypeName { get; set; }

        public SpaceMission(dynamic item)
        {
            // Initialize the object from the json object
            Description = Convert.ToString(item.description);
            ID = Convert.ToInt32(item.id);
            Name = Convert.ToString(item.name);
            TypeName = Convert.ToString(item.typename);
        }
    }
}
