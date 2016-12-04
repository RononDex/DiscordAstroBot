using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Reactions
{
    public static class Reactions
    {
        public static string GetReaction(string input)
        {
            // Find all possible answers
            var possibleAnswersList = ReactionDict.Where(x => x.Key.Contains(input)).Select(x => x.Value).ToList();
            var possibleAnswers = new List<string>();
            foreach (var item in possibleAnswersList)
                possibleAnswers.AddRange(item);

            // If no reaction defined
            if (possibleAnswers.Count == 0)
            {
                possibleAnswersList = ReactionDict.Where(x => x.Key.Contains("Unknown")).Select(x => x.Value).ToList();

                possibleAnswers = new List<string>();
                foreach (var item in possibleAnswersList)
                    possibleAnswers.AddRange(item);
            }

            // Select one answer by random
            var random = new Random();
            var reaction = possibleAnswers[random.Next(possibleAnswers.Count)];

            return reaction;
        }

        static Dictionary<string[], string[]> ReactionDict { get; set; } = new Dictionary<string[], string[]>()
        {
            { new string[] { "" },                                          new string[] { "How can I help you?", "At your service", "Yes?", "Hi!" } },
            { new string[] { "hi", "Hi", "Hello", "hello" },                new string[] { "Hi!", "Hello", "Hi back" } },
            { new string[] { "Unknown"},                                    new string[] { "I am sorry, I don't know how to respond to that" } }
        };
    }
}
