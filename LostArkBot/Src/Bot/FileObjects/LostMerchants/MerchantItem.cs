using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.LostMerchants;

// ReSharper disable once ClassNeverInstantiated.Global
public class MerchantItem
{
    [JsonPropertyName("name")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Name { get; set; }

    [JsonPropertyName("rarity")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Rarity Rarity { get; set; }
}