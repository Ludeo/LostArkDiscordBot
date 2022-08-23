using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.LostMerchants;

public class MerchantInfo
{
    [JsonPropertyName("Region")]
    public string Region { get; init; }
}