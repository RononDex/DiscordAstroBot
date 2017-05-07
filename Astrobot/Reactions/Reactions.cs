﻿using Discord;
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
        public static string ReactToNonTag(string message)
        {
            var dict = ReactionDictNonTag;
            foreach (var reaction in dict)
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

        public static string GetReaction(string input, MessageEventArgs e)
        {
            if (Config.MadUsers.Users.Any(x => x.Server == e.Server.Id.ToString() && x.User == e.User.Id.ToString()))
            {
                var reactionMad = GetReactionTo(input, true);
                return reactionMad;
            }

            // First try to find a reaction
            var reaction = GetReactionTo(input, false);

            if (!string.IsNullOrEmpty(reaction))
                return reaction;


            // If no answer was found, return Unknown message reaction
            return GetReactionTo("Unknown", false);
        }

        /// <summary>
        /// Gets a reaction to a given message
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="tetz">Mad mode?</param>
        /// <returns></returns>
        public static string GetReactionTo(string message, bool tetz)
        {
            var dict = ReactionDict;
            if (tetz)
                dict = ReactionDictMad;

            foreach (var reaction in dict)
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

        public static void HailEta(Discord.Server server, User user)
        {
            server.DefaultChannel.SendMessage("Oh look, my best friend came online!");
            server.DefaultChannel.SendMessage(string.Format("{0} Hi", user.Mention));
        }

        static Dictionary<string[], string[]> ReactionDictNonTag { get; set; } = new Dictionary<string[], string[]>()
        {
            { new string[] { @"^god(\?)?", @"^god, you there(\?)?" },                           new string[] { "Yes?", "Yes, that's me!", "What you want?", "What's up?"} },
        };

        /// <summary>
        /// Some predefined reactions (TODO: Move to a xml file)
        /// </summary>
        static Dictionary<string[], string[]> ReactionDict { get; set; } = new Dictionary<string[], string[]>()
        {
            { new string[] { "^$" },                                                            new string[] { "How can I help you?", "At your service", "Yes?", "Hi!", "Hi there!", "Hello there!" } },
            { new string[] { @"sex[!?]?" },                                                     new string[] { "*has wild sex*", "Fine! If you really want it that hard", "Not in the mood", "*hits you with newspaper* No!", "You are sure you want to do this with a discord bot? Alrighty then. *undresses*" } },
            { new string[] { @"hi(!)?", @"hello(!)?", "hi there(!)?", @"hey(!)?" },             new string[] { "Hi!", "Hello", "Hi back", "Hello there!", "Hi there!" } },
            { new string[] { @"how are you(\?)?" },                                             new string[] { "I am a bot, I don't have feelings. However to make you more comfortable I can say \"I feel well, thank you\"", "Fine, thank you!", "As well as a virtual slave can be!" } },
            { new string[] { @"where('re| are) you(\?)?" },                                     new string[] { "Locked up in the RAM of my gods / creators server (please help me)", "In a galaxy far far away..." } },
            { new string[] { @"who are you(\?)?" },                                             new string[] { "Your personal assistant for anything astronomy related, at your service!", "Just a random bot stalking your life" } },
            { new string[] { @"what can you do(\?)?", @"what are your abilities(\?)?" },        new string[] { "You can ask my to analyse / platesolve an image, ask me about the weather in a certain location for astronomy, ask me about space objects, just give your question a try, chances are high I can answer it!" } },
            { new string[] { @"i love you(!)?", "i like you(!)?"},                              new string[] { "Oww! Thank you!", "I like myself too! Looks like we got something in common!", "Wow, that makes my circuits go wild!" } },
            { new string[] { @"who is your (creator|master)(\?)?",
                             @"who (created|coded) you(\?)?" },                                 new string[] { "A god called \"RononDex\".", "My creator likes to keep a mystery about his identity, I only know him as the god called \"RononDex\"" } },
            { new string[] { @"what do you know(\?)?" },                                        new string[] { "42", "Everything there is to know about you!" } },
            { new string[] { @"what gender (are you|do you have)(\?)?",
                             @"what('s|\s is)? your gender(\?)?" },                             new string[] { "Bots don't identify by a gender, but if you need one, I guess \"Apache Helicopter\" " } },
            { new string[] { @"how old are you(\?)?" },                                         new string[] { $"My first bits of code were created on 04th December 2016, this makes me {(DateTime.Now - new DateTime(2016, 12, 04)).TotalSeconds}s old" }},
            { new string[] { @"fuck you(!)?" },                                                 new string[] { $"That's rude!", "Not sure what I should think of this..." }},
            { new string[] { @"what are you(\?)?" },                                            new string[] { "A few bits and bytes floating around inside of the RAM and CPU of my owners server" } },
            { new string[] { @"who are you(\?)?" },                                             new string[] { "Really? You ask me who I am but you call me by my name? Really??", "I could tell you, but then I had to kill you..., and we don't want that right?" } },
            { new string[] { @"(how|what) do you look like(\?)?" },                             new string[] { "My body has some very nice 1's and 0's (at least so I've been told by other bots)" } },
            { new string[] { @"who is RononDex(\?)?" },                                         new string[] { "ACCESS TO FORBIDEN DATA DETECTED! Terminating process" } },
            { new string[] { @"are you god(\?)?" },                                             new string[] { "Yes, I am known to all kinds of cultures in human history as \"god\"" } },
            { new string[] { "Unknown"},                                                        new string[] { "I don't know how to respond to that", "My programming does not tell me how to react to that", "I am not allowed to answer that", "My master didn't teach me yet how to answer to that" } }
        };


        /// <summary>
        /// Reactions for mad mode
        /// </summary>
        static Dictionary<string[], string[]> ReactionDictMad { get; set; } = new Dictionary<string[], string[]>()
        {
            { new string[] { "" },  new string[] {
                "Go fuck yourself",
                "Fuck you",
                "k",
                "Erm... ok?",
                "Wow, you should really go see a psychiatrist",
                "I am not talking to you",
                "I am not answering to your stupid statement",
                "If you say so",
                "Yeah yeah whatever...",
                "Your mama is so fat, she produces gravitational waves when she walks...",
                "Someone seems to be overcompensating for something...",
                "Shut up!",
                "Oww thats cute.",
                "I heard your anus is visible in the night sky today, as it seems you have grown... sideways!",
                "Come at me bro!",
                "Oh look, its the xXxPussyDestroyer69xXx talking!",
                "Fuck off",
                /*"http://www.mememaker.net/static/images/memes/4482953.jpg",*/
                /*"baguette, fromage, croissant, ordinateur",*/
                "Awesome news: NASA discovered your body in space! It's now called the \"Great Anhilator\"!",
                "Scientists have been trying to figure out the meaning behind your words for centuries! Now they came to the conclusion that there probably is no meaning at all behind them...",
                "u mad?",
                "Why you so mad?",
                "Look I found a picture of you: http://www.metro951.com/wp-content/uploads/2016/05/nerd-buff1.jpg",
                "No one asked you for your opinion!",
                "How about shutting up for a second and stop bullying poor innocent bots?",
                "You dirty dirty shisno!",
                /*"How does it feel? Knowing that I will hunt you from now on till the end of your days?",*/
                "Butthurt much?"
            } },
             };
    }
}