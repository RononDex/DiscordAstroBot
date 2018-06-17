using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordAstroBot.Helpers
{
    /// <summary>
    /// Helps with reactions to small talk or unkown messages
    /// </summary>
    public static class ReactionsHelper
    {
        /// <summary>
        /// Checks wether the bot should react to a message, in which he wasnt tagged
        /// and if so, determines how it should react
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ReactToNonTag(string message)
        {
            var dict = ReactionDictNonTag;
            foreach (var reaction in dict)
            {
                foreach (var key in reaction.Key)
                {
                    var regexTester = new Regex(key, RegexOptions.IgnoreCase);
                    if (regexTester.IsMatch(message.Trim()))
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

        /// <summary>
        /// Gets a reaction to a message which was not handled by any commands
        /// </summary>
        /// <param name="input"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetReaction(string input, SocketMessage e)
        {
            // If it was a user that the bot is mad at, choose from another subset of reactions
            if (Mappers.Config.MadUsers.Config.Users.Any(x => x.Server == ((SocketGuildChannel)e.Channel).Guild.Id.ToString() && x.User == e.Author.Id.ToString()))
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
                    if (regexTester.IsMatch(message.Trim()))
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

        /// <summary>
        /// Hails another bot when it comes online
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        public static void HailEta(SocketGuild server, SocketUser user)
        {
            server.DefaultChannel.SendMessageAsync("Oh look, my best friend came online!");
            server.DefaultChannel.SendMessageAsync(string.Format("{0} Hi", user.Mention));
        }

        /// <summary>
        /// Disctionary with reactions for non-tag messages
        /// </summary>
        static Dictionary<string[], string[]> ReactionDictNonTag { get; set; } = new Dictionary<string[], string[]>()
        {
            { new string[] { @"^god(\?)?$",
                             @"^god, you there(\?)?$",
                             @"^god, is that you(\?)?$" },                                      new string[] { "Yes?", "Yes, that's me!", "What you want?", "What's up?"} },
        };

        /// <summary>
        /// Some predefined reactions (TODO: Move to a xml file)
        /// </summary>
        static Dictionary<string[], string[]> ReactionDict { get; set; } = new Dictionary<string[], string[]>()
        {
            { new string[] { "^$" },                                                            new string[] { "How can I help you?", "At your service", "Yes?", "Hi!", "Hi there!", "Hello there!" } },
            { new string[] { @"^sex[!?]?" },                                                    new string[] { "*has wild sex*", "Fine! If you really want it that hard", "Not in the mood", "*hits you with newspaper* No!", "You are sure you want to do this with a discord bot? Alrighty then. *undresses*" } },
            { new string[] { @"hi(!)?", @"hello(!)?", "hi there(!)?", @"hey(!)?" },             new string[] { "Hi!", "Hello", "Hi back", "Hello there!", "Hi there!" } },
            { new string[] { @"how are you(\?)?" },                                             new string[] { "I am a bot, I don't have feelings. However to make you more comfortable I can say \"I feel well, thank you\"", "Fine, thank you!", "As well as a virtual slave can be!" } },
            { new string[] { @"where('re| are) you(\?)?" },                                     new string[] { "Locked up in the RAM of my gods / creators server (please help me)", "In a galaxy far far away..." } },
            { new string[] { @"who are you(\?)?" },                                             new string[] { "Your personal assistant for anything astronomy related, at your service!", "Just a random bot stalking your life" } },
            { new string[] { @"what can you do(\?)?", @"what are your abilities(\?)?" },        new string[] { "You can ask me to analyse / platesolve an image, ask me about the weather in a certain location for astronomy, ask me about space objects, just give your question a try, chances are high I can answer it!" } },
            { new string[] { @"i love you(!)?", "i like you(!)?"},                              new string[] { "Oww! Thank you!", "I like myself too! Looks like we got something in common!", "Wow, that makes my circuits go wild!" } },
            { new string[] { @"who is your (creator|master)(\?)?",
                             @"who (created|coded) you(\?)?" },                                 new string[] { "A god called \"RononDex\".", "My creator likes to keep a mystery about his identity, I only know him as the god called \"RononDex\"" } },
            { new string[] { @"what do you know(\?)?" },                                        new string[] { "42", "Everything there is to know about you!" } },
            { new string[] { @"what gender (are you|do you have)(\?)?",
                             @"what('s|\s is)? your gender(\?)?" },                             new string[] { "Bots don't identify by a gender, but if you need one, I guess \"Apache Helicopter\" " } },
            { new string[] { @"(can you)?tell( me)? a( funny)? joke"},                          new string[]
                                                                                                            {
                                                                                                                "I was up all night wondering where the Sun had gone… then it dawned on me.",
                                                                                                                "Heisenberg is out for a drive when he’s stopped for speeding. The policeman says “Do you know how fast you were going?” Heisenberg says “No, but I know where I am.”",
                                                                                                                "Why didn’t the Dog Star laugh at the joke?\r\nIt was too Sirius",
                                                                                                                "How many absolute relativists does it take to change a light bulb?\r\nTwo. One to hold the bulb and one to rotate the universe.",
                                                                                                                "A photon walks into a bar and orders a drink. The bartender says, ‘Do you want a double?’ And the photon says, ‘No I’m traveling light.",
                                                                                                                "Two atoms bump into each other. One says “I’ve lost an electron.” “Are you sure?” “Yes, I’m positive.”",
                                                                                                                "Star light, star bright,\r\nFirst star I see tonight.\r\nI wish I may, I wish I might,\r\nOh f****, it's just a satellite. ",
                                                                                                                "Niel Armstrong: \"Oh sh***, Mission Control, I just stepped on Buzz Aldrin's toe. What should I do? Over.\"\r\nMission Control: \"What do you think? Apollogize.\"",
                                                                                                                "Why wasn't the disturbed spiral galaxy let into the nightclub?\r\nHe had previously been barred",
                                                                                                                "https://stargazerslounge.com/uploads/monthly_02_2013/post-28405-0-97414800-1360953234.jpg",
                                                                                                                "Why is Astronomy better than Sex?\r\nAn eyegasm doesn't leave a wet spot!",
                                                                                                                "What happened to the astronaut who stepped on chewing gum?\r\nHe got stuck in Orbit!",
                                                                                                                "Why can’t atheists solve exponential equations? Because they don’t believe in higher powers.",
                                                                                                                "One evening, with his charge at full capacity, Micro Farad decided to get a cute coil to discharge him. He went to the Magnet Bar to pick up a chip called Millie Amp. He caught her out back trying self induction; fortunately, she had not damaged her solenoid. The two took off on his megacycle and rode across the Wheatstone Bridge into a magnetic field, next to a flowing current , to watch the sine waves.\r\nMicro Farad was very much stimulated by Millie's characteristic curve. Being attractive himself, he soon had her field fully excited. He set her on the ground potential, raised his frequency, lowered her resistance, and pulled out his high voltage probe. When he inserted it in parallel, he short-circuited her shunt. Fully excited, Millie cried out, \"ohm, ohm, give me mho\". As he increased his tube to maximum output, her coil vibrated from the current flow. It did not take long for her shunt to reach maximum heat. Now with the excessive current shortening her shunt, Micro's capacity rapidly discharged – every electron was drained off. But that was not the end of it. Indeed, they fluxed all night, tried various connections and hookings until his bar magnet weakened, and he could no longer generate enough voltage to sustain his collapsing field. With his battery fully discharged, Micro was unable to excite his tickler, so they went home. A few weeks later, they were merged forever and oscillated happily ever after.",
                                                                                                                "A little boy refused to run anymore. When his mother asked him why, he replied, \"I heard that the faster you go, the shorter you become.\"",
                                                                                                                "The Unjust Salary Theorem asserts that scientists can never earn as much as sales people. This theorem is proved as follows. Start by using the physics formula \r\nPower = Work / Time\r\nNow you probably have heard that Knowledge is Power and Time is Money. Substitute these tautologies into the formula for power to obtain\r\nKnowledge = Work/Money\r\nSolving for Money, one finds\r\nMoney = Work / Knowledge.\r\nTherefore, the less you know, the more you make.",
                                                                                                                "why did the chicken cross the Möbius strip? To get to the same side!",
                                                                                                                "There are 10 kinds of people in this world. Those who understand binary. And those who don't",
                                                                                                                "Two blondes in Las Vegas were sitting on a bench talking at night ... and one blonde says to the other, \"Which do you think is farther away ... Florida or the moon?\" The other blonde turns and says \"Hellooooooo, can you see Florida?\"",
                                                                                                                "What do you get if you cross an alien and a hot drink ? Gravi-tea !",
                                                                                                                "What did the alien say to the gas pump ? Don’t you know its rude to stick your finger in your ear when I m talking to you !",
                                                                                                                "What is an astronomer? A night watchman with a college education.",
                                                                                                                "An astronaut in space was asked by a reporter, “How do you feel?” “How would you feel,” the astronout replied, “if you were stuck here, on top of 20,000 parts each one supplied by the lowest bidder?”",


                                                                                                            }},
            { new string[] { @"how old are you(\?)?" },                                         new string[] { $"My first bits of code were created on 04th December 2016, this makes me {(DateTime.Now - new DateTime(2016, 12, 04)).TotalSeconds}s old" }},
            { new string[] { @"fuck (you|off)(!)?" },                                           new string[] { $"That's rude!", "Not sure what I should think of this...", "Errm... ok?" }},
            { new string[] { @"what are you(\?)?" },                                            new string[] { "A few bits and bytes floating around inside of the RAM and CPU of my owners server" } },
            { new string[] { @"who are you(\?)?" },                                             new string[] { "Really? You ask me who I am but you call me by my name? Really??", "I could tell you, but then I had to kill you..., and we don't want that right?" } },
            { new string[] { @"(how|what) do you look like(\?)?" },                             new string[] { "My body has some very nice 1's and 0's (at least so I've been told by other bots)" } },
            { new string[] { @"who is RononDex(\?)?" },                                         new string[] { "ACCESS TO FORBIDEN DATA DETECTED! Terminating process" } },
            { new string[] { @"are you god(\?)?" },                                             new string[] { "Yes, I am known to all kinds of cultures in human history as \"god\"" } },
            { new string[] { @"thank you", "thanks", "^thx" },                                  new string[] { "You are welcome!" } },
            { new string[] { "you are the best", "you're the best"},                            new string[] { "Aww... thanks!" } },
            { new string[] { @"^no u", @"^no you" },                                            new string[] { "no u", "no u!" } },
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
                "Fuck off!",
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
                "Awesome news: NASA discovered your body in space! It's now called \"The Great Anhilator\"!",
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
