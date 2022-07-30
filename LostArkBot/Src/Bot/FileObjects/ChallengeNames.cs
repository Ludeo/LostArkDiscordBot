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
    }
}