using Discord;
using DiscordAstroBot.Helpers;
using DiscordAstroBot.Mappers.Simbad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordAstroBot.Objects.Simbad;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    public class Simbad : Command
    {
        public override string CommandName => "Simbad";

        public override string[] CommandSynonyms => new string[] {
            @"what do you know about (?'AstroObject'.*\w)(\?)?",
            @"what (is|are) the (magnitude|magnitudes|brightness|brightnes|fluxes|flux) of (?'MagAstroObject'.*\w)(\?)?",
            @"how bright is (?'MagAstroObject'.*\w)(\?)?",
            @"what is the distance of (?'DistAstroObject'.*\w)(\?)?",
            @"how far (away )?is (?'DistAstroObject'\w*)( away)?( from earth| from us)?(\')?",
            @"what is (?'AstroObject'.*\w)(\?)?"
        };

        public override string Description => "Access data from SIMBAD. Usage: \r\n```    @Astro Bot What is M31\r\n    @Astro Bot How far away is M31```";

        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            await recievedMessage.Channel.SendMessageAsync("Querying the SIMBAD database, please wait...");

            // Output full information for "AstroObject"
            if (matchedMessage.Groups["AstroObject"].Success)
            {
                var info = SimbadQuery.GetAstronomicalObjectInfo(matchedMessage.Groups["AstroObject"].Value);

                if (info == null)
                {
                    await recievedMessage.Channel.SendMessageAsync($"Could not find any object matching your search \"{matchedMessage.Groups["AstroObject"].Value}\" in the SIMBAD database");
                    return true;
                }

                var obj = AstronomicalObjectInfo.FromSimbadResult(info);
                await recievedMessage.Channel.SendMessageAsync($"This is what I found in the SIMBAD database:\r\n" +
                                    $"```\r\n" +
                                    $"Main Identifier: {obj.Name}\r\n" +
                                    $"MainType: {obj.ObjectType}\r\n" +
                                    $"Coordinates:\r\n{obj.Coordinates}\r\n\r\n" +
                                    $"Magntidues:\r\n{string.Join("\r\n", obj.Magntiudes)}\r\n\r\n" +
                                    $"Distance: \r\n{string.Join("\r\n", obj.DistanceMeasurements)}\r\n\r\n" +
                                    $"Radial velocity:\r\n{obj.RadialVelocity}\r\n\r\n" +
                                    $"Parallax:\r\n{obj.Parallax}\r\n\r\n" +
                                    $"Proper motion:\r\n{obj.ProperMotion}\r\n\r\n" +
                                    $"```\r\n");

                await recievedMessage.Channel.SendMessageAsync($"```\r\n" +
                                    $"SecondaryTypes:\r\n{string.Join(", ", obj.SecondaryTypes.Select(x => x.Replace("\n", "").Replace("\r", "")))}\r\n\r\n" +
                                    $"Also known as: \r\n{string.Join(", ", obj.AlsoKnownAs)}\r\n\r\n" +
                                    $"```\r\n");
            }

            // Output magnitudes
            if (matchedMessage.Groups["MagAstroObject"].Success)
            {
                var info = SimbadQuery.GetAstronomicalObjectInfo(matchedMessage.Groups["MagAstroObject"].Value);

                if (info == null)
                {
                    await recievedMessage.Channel.SendMessageAsync($"Could not find any object matching your search \"{matchedMessage.Groups["MagAstroObject"].Value}\" in the SIMBAD database");
                    return true;
                }

                var obj = AstronomicalObjectInfo.FromSimbadResult(info);
                if (obj.Magntiudes.Count == 0)
                {
                    await recievedMessage.Channel.SendMessageAsync($"The object {matchedMessage.Groups["MagAstroObject"].Value} was found in the database, but no fluxes are known");
                    return true;
                }

                await recievedMessage.Channel.SendMessageAsync($"These are the magnitudes I found for {matchedMessage.Groups["MagAstroObject"].Value}" +
                                      $"```\r\n" +
                                      $"{string.Join("\r\n", obj.Magntiudes)}\r\n\r\n" +
                                      $"```\r\n");
            }

            // Output distance
            if (matchedMessage.Groups["DistAstroObject"].Success)
            {
                var info = SimbadQuery.GetAstronomicalObjectInfo(matchedMessage.Groups["DistAstroObject"].Value);

                if (info == null)
                {
                    await recievedMessage.Channel.SendMessageAsync($"Could not find any object matching your search \"{matchedMessage.Groups["DistAstroObject"].Value}\" in the SIMBAD database");
                    return true;
                }

                var obj = AstronomicalObjectInfo.FromSimbadResult(info);
                if (obj.DistanceMeasurements.Count == 0)
                {
                    await recievedMessage.Channel.SendMessageAsync($"The object {matchedMessage.Groups["DistAstroObject"].Value} was found in the database, but no distance measurements were found");
                    return true;
                }

                await recievedMessage.Channel.SendMessageAsync($"The object {matchedMessage.Groups["DistAstroObject"].Value} is approximatly `{obj.DistanceMeasurements[0].Distance} {obj.DistanceMeasurements[0].Unit} ± {obj.DistanceMeasurements[0].ErrPlus.Replace("+", "")}` away from earth.");
            }

            return true;
        }
    }
}
