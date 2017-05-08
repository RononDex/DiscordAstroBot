using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscordAstroBot.XmlSerialization
{
    /// <summary>
    /// Allows to load and save xml files
    /// </summary>
    public static class XmlStateController
    {
        /// <summary>
        /// lock object used to manage multi threading
        /// </summary>
        private static object lockObj = null;

        public static T LoadObject<T>(string xmlFile)
        {
            Log<DiscordAstroBot>.Info($"Loading xml file {xmlFile}");
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(new StringReader(File.ReadAllText(xmlFile)));
        }

        public static void SaveObject<T>(T obj, string xmlFile)
        {
            // Multi-thread safety, only one thread at a time should write to the xml file
            lock (lockObj)
            {
                Log<DiscordAstroBot>.Info($"Saving xml file {xmlFile}");
                var serializer = new XmlSerializer(typeof (T));
                using (var fileStream = new FileStream(xmlFile, FileMode.OpenOrCreate))
                {
                    serializer.Serialize(fileStream, obj);
                }
            }
        }
    }
}
