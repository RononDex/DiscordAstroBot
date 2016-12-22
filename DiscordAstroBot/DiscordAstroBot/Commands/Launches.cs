using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace DiscordAstroBot.Commands
{
    class Launches : Command
    {
        public override string CommandName { get { return "Launches"; } }

        public override string[] CommandSynonyms
        {
            get
            {
                return new string[] {
                    @"what agencies start with (?'AgencySearchName'.*\w)(\?)?"
                };
            }
        }

        public override void MessageRecieved(Match matchedMessage, MessageEventArgs e)
        {
            if (matchedMessage.Groups["AgencySearchName"] != null)
            {
                var agencies = Helpers.LaunchLibraryAPIHelper.FindAgenciesByName(matchedMessage.Groups["AgencySearchName"].Value);
                var answer = string.Format("Following agencies are known to me, that match your search:\r\n```");
                foreach (var agency in agencies)
                {
                    answer = string.Format("{0}\r\n{1}", answer, agency);
                }

                answer = string.Format("{0}\r\n```", answer);

                e.Channel.SendMessage(answer);
            }
        }
    }
}
