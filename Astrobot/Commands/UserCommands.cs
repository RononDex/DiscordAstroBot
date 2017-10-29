using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordAstroBot.Commands
{
    /// <summary>
    /// User specific commands
    /// </summary>
    public class UserCommands : Command
    {
        /// <summary>
        /// The name of the command
        /// </summary>
        public override string CommandName => "UserCommands";

        /// <summary>
        /// Synonyms that trigger the command
        /// </summary>
        public override string[] CommandSynonyms => new[] {
            "give me(( the)? role)? (?'GiveRoleName'.*)",
            "remove(( the)? role)?(from me)? (?'RemoveRoleName'.*)"
        };

        /// <summary>
        /// Event when message was recieved
        /// </summary>
        /// <param name="matchedMessage"></param>
        /// <param name="recievedMessage"></param>
        /// <returns></returns>
        public override async Task<bool> MessageRecieved(Match matchedMessage, SocketMessage recievedMessage)
        {
            // Assign role to user
            if (matchedMessage.Groups["GiveRoleName"].Success)
            {
                var role = matchedMessage.Groups["GiveRoleName"].Value;
                var server = (recievedMessage.Channel as SocketGuildChannel).Guild;

                var allowedRoles = Mappers.Config.ServerConfig.Config.Servers.FirstOrDefault(x => x.ServerID == server.Id)
                    .Configs.FirstOrDefault(x => x.Key == "UserAssignableRoles").Value.Split(';');

                // If group not allowed or not exists on server, exit with error
                if (!allowedRoles.Any(x => x.ToLower() == role.ToLower()) || !server.Roles.Any(x => x.Name.ToLower() == role.ToLower()))
                {
                    await recievedMessage.Channel.SendMessageAsync($"The role \"{role}\" which you requested does either not exist or I have no permission to assign it to you");
                    Log<DiscordAstroBot>.Warn($"User {recievedMessage.Author.Username} tried assinging himself the invalid role {role} on server {server.Name}");
                    return true;
                }

                // Assign role to user
                await (recievedMessage.Author as SocketGuildUser).AddRoleAsync(server.Roles.FirstOrDefault(x => x.Name.ToLower() == role.ToLower()));
                Log<DiscordAstroBot>.InfoFormat($"Assigned role {role} to user {recievedMessage.Author.Username} on server {server.Name}");
                await recievedMessage.Channel.SendMessageAsync($"Done, you are now assigned the role {role}");
            }

            // Remove a user role
            if (matchedMessage.Groups["RemoveRoleName"].Success)
            {
                var role = matchedMessage.Groups["RemoveRoleName"].Value;
                var server = (recievedMessage.Channel as SocketGuildChannel).Guild;

                var socketUser = recievedMessage.Author as SocketGuildUser;
                if (socketUser.Roles.Any(x => x.Name.ToLower() == role.ToLower()))
                {
                    await socketUser.RemoveRoleAsync(socketUser.Roles.FirstOrDefault(x => x.Name.ToLower() == role.ToLower()));
                    Log<DiscordAstroBot>.Info($"Removed the role {role} from user {recievedMessage.Author.Username} on server {server.Name}");
                    await recievedMessage.Channel.SendMessageAsync($"Role {role} removed!");
                }
            }

            return true ;
        }
    }
}
