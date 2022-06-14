using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.LostMerchants
{
    public class MerchantInfo
    {
        [JsonPropertyName("Region")]
        public string Region { get; set; }
    }
}
