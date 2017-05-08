using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Mappers.Simbad
{
    /// <summary>
    /// Represents a rsult from the SIMBAD query
    /// </summary>
    public class SimbadResult
    {
        public SimbadResult(string resultText)
        {
            // Parse Text into sections
            var lines = resultText.Split(new[] { "\n" }, StringSplitOptions.None);

            var curSection = "";
            var curSectionText = "";
            foreach (var line in lines)
            {
                // If we find a new section
                if (line.StartsWith("[[") && line.EndsWith("]]"))
                {
                    if (string.IsNullOrEmpty(curSectionText))
                        curSectionText = null;

                    Sections.Add(curSection, curSectionText);
                    curSection = line.Substring(2, line.Length - 4);
                    curSectionText = string.Empty;
                    continue;
                }

                curSectionText = string.Format("{0}{1}\r\n", curSectionText, line);
            }

        }

        public Dictionary<string, string> Sections { get; set; } = new Dictionary<string, string>();
    }
}
