using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordAstroBot.Reactions
{
    public static class Reactions
    {
        public static string GetReaction(string input)
        {
            // First try to find a reaction
            var reaction = GetReactionTo(input);

            if (!string.IsNullOrEmpty(reaction))
                return reaction;


            // If no answer was found, return Unknown message reaction
            return GetReactionTo("Unknown");
        }

        public static string GetReactionTo(string message)
        {
            foreach (var reaction in ReactionDict)
            {
                foreach (var key in reaction.Key)
                {
                    var regexTester = new Regex(key, RegexOptions.IgnoreCase);
                    if (regexTester.IsMatch(message))
                    {
                        // Select one answer by random
                        var random = new Random();
                        var result = reaction.Value[random.Next(reaction.Value.Length)];
                        return result;
                    }
                }
            }

            return null;            
        }

        static Dictionary<string[], string[]> ReactionDict { get; set; } = new Dictionary<string[], string[]>()
        {
            { new string[] { "^$" },                                                        new string[] { "How can I help you?", "At your service", "Yes?", "Hi!" } },
            { new string[] { @"hi(\!)?", @"hello(\!)?" },                                   new string[] { "Hi!", "Hello", "Hi back" } },
            { new string[] { @"how are you(\?)?" },                                         new string[] { "I am a bot, I don't have feelings. However to make you more comfortable I can say \"I feel well, thank you\"", "Fine, thank you!", "As well as a virtual slave can be!" } },
            { new string[] { @"where are you(\?)?" },                                       new string[] { "Locked up in the RAM of my gods / creators server (please help me)", "In a galaxy far far away..." } },
            { new string[] { @"who are you(\?)?" },                                         new string[] { "Your personal assistant for anything astronomy related, at your service!" } },
            { new string[] { @"what can you do(\?)?", @"what are your abilities(\?)?" },    new string[] { "You can ask my to analyse / platesolve an image, ask me about the weather in a certain location for astronomy, ask me about space objects, just give your question a try, chances are high I can answer it!" } },
            { new string[] { @"i love you", "i like you"},                                  new string[] { "Oww! Thank you!", "I like myself too! Looks like we got something in common!" } },
            { new string[] { @"what do you know(\?)?" },                                    new string[] { "42", "Everything there is to know about you!" } },
            { new string[] { "Unknown"},                                                    new string[] { "I am sorry, I don't know how to respond to that", "My programming does not tell me how to react to that", "I am not allowed to answer that" } }
        };
    }
}
