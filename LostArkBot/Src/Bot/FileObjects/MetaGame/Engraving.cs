using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.MetaGame
{
    public class Engraving
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("desc")]
        public string[] Descriptions { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        public int Value { get; set; } = 0;

        [JsonPropertyName("penalty")]
        public bool Penalty { get; set; } = false;
    }
}