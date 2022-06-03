using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    internal class MetaGameCharacter
    {
        [JsonPropertyName("pcClassName")]
        public string ClassName { get; set; }

        [JsonPropertyName("maxItemLevel")]
        public double ItemLevel { get; set; }

        [JsonPropertyName("jsonData")]
        public string JsonData { get; set; }
    }
}
