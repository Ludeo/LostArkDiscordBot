using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    public class ChallengeGuardian
    {
        [JsonPropertyName("guardianname")]
        public string GuardianName { get; set; }
    }
}