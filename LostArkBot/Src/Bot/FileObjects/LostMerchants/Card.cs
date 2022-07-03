using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.LostMerchants
{
    public class Card
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }
    }
}