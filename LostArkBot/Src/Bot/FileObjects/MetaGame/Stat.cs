using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.MetaGame
{
    internal class Stat
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
