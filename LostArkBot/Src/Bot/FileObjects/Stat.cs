using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    internal class Stat
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
