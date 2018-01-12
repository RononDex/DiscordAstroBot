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

        public override CommandSynonym[] CommandSynonyms => new [] {
            new CommandSynonym() { Synonym = @"what do you know about (?'AstroObject'.*\w)(\?)?" },
            new CommandSynonym() { Synonym = @"what (is|are) the (magnitude|magnitudes|brightness|brightnes|fluxes|flux) of (?'MagAstroObject'.*\w)(\?)?" },
            new CommandSynonym() { Synonym = @"how bright is (?'MagAstroObject'.*\w)(\?)?" },
            new CommandSynonym() { Synonym = @"what is the distance of (?'DistAstroObject'.*\w)(\?)?" },
            new CommandSynonym() { Synonym = @"how far (away )?is (?'DistAstroObject'\w*)( away)?( from earth| from us)?(\')?" },
            new CommandSynonym() { Synonym = @"what is (?'AstroObject'.*\w)(\?)?" },
            new CommandSynonym() { Synonym = @"how big is (?'SizeAstroObject'.*\w)(\?)?" }
        };

        public override string Description => "Access data from SIMBAD. Usage: \r\n```    @Astro Bot What is M31\r\n    @Astro Bot How far away is M31\r\n    @Astro Bot how big is M31\r\n```";

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

                var angularDistanceText = obj.AngularDimension != null
                    ? $"{obj.AngularDimension.XSize}arcmin x {obj.AngularDimension.YSize}arcmin, rotated at {obj.AngularDimension.Rotation}°"
                    : string.Empty;

                await recievedMessage.Channel.SendMessageAsync($"This is what I found in the SIMBAD database:\r\n" +
                                    $"```python\r\n" +
                                    $"Main Identifier: {obj.Name}\r\n" +
                                    $"MainType: {obj.ObjectType}\r\n" +
                                    $"Coordinates:\r\n{obj.Coordinates}\r\n\r\n" +
                                    $"Angular Dimensions: {angularDistanceText}\r\n\r\n" +
                                    $"Magntidues:\r\n{string.Join("\r\n", obj.Magntiudes)}\r\n\r\n" +
                                    $"Distance: \r\n{string.Join("\r\n", obj.DistanceMeasurements)}\r\n\r\n" +
                                    $"adial velocity:\r\n{obj.RadialVelocity}\r\n\r\n" +
                                    $"Parallax:\r\n{obj.Parallax}\r\n\r\n" +
                                    $"Proper motion:\r\n{obj.ProperMotion}\r\n\r\n" +
                                    $"```\r\n");

                await recievedMessage.Channel.SendMessageAsync($"```python\r\n" +
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

            // What's the angular size of an object?
            if (matchedMessage.Groups["SizeAstroObject"].Success)
            {
                var info = SimbadQuery.GetAstronomicalObjectInfo(matchedMessage.Groups["SizeAstroObject"].Value);

                if (info == null)
                {
                    await recievedMessage.Channel.SendMessageAsync($"Could not find any object matching your search \"{matchedMessage.Groups["DistAstroObject"].Value}\" in the SIMBAD database");
                    return true;
                }

                var obj = AstronomicalObjectInfo.FromSimbadResult(info);
                if (obj.AngularDimension == null)
                {
                    await recievedMessage.Channel.SendMessageAsync($"The object {matchedMessage.Groups["SizeAstroObject"].Value} was found in the database, but no angular size measurements were found");
                    return true;
                }

                await recievedMessage.Channel.SendMessageAsync($"The object {matchedMessage.Groups["SizeAstroObject"].Value} has an angular size of:\r\n```\r\n{obj.AngularDimension.XSize}arcmin x {obj.AngularDimension.YSize}arcmin\r\nat an angle of {obj.AngularDimension.Rotation}°\r\n```");
            }

            return true;
        }
    }
}
