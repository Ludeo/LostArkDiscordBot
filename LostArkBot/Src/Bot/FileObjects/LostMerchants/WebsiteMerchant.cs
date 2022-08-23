using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.LostMerchants;

// ReSharper disable once ClassNeverInstantiated.Global
public class WebsiteMerchant
{
    [JsonPropertyName("id")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Id { get; set; }

    [JsonPropertyName("name")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Name { get; set; }

    [JsonPropertyName("zone")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Zone { get; set; }

    [JsonPropertyName("card")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public MerchantItem Card { get; set; }

    [JsonPropertyName("rapport")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public MerchantItem Rapport { get; set; }

    [JsonPropertyName("votes")]
    public int Votes { get; set; }
}