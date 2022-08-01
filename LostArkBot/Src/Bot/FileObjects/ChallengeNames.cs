using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    public class ChallengeNames
    {
        [JsonPropertyName("guardians")]
        public List<string> ChallengeGuardian { get; set; }

        [JsonPropertyName("abyss")]
        public List<string> ChallengeAbyss { get; set; }

        public ChallengeNames()
        {
            ChallengeGuardian = new List<string>() { "Guardian 1", "Guardian 2", "Guardian 3" };
            ChallengeAbyss = new List<string>() { "Abyss 1", "Abyss 2" };
        }
    }
}