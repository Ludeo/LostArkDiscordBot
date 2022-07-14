using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.LostMerchants
{
    public class Merchant
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("zone")]
        public string Zone { get; set; }

        [JsonPropertyName("card")]
        public MerchantItem Card { get; set; }

        [JsonPropertyName("rapport")]
        public MerchantItem Rapport { get; set; }

        [JsonPropertyName("votes")]
        public int Votes { get; set; }
    }
}