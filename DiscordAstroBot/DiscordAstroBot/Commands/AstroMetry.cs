using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordAstroBot.Commands
{
    public class AstroMetry : Command
    {
        public override string CommandName { get { return "Astrometry"; } }

        public override void MessageRecieved(string message, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
