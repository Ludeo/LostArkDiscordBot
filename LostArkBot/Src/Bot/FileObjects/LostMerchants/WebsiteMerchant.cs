using System.Collections.Generic;
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

    [JsonPropertyName("cards")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public List<MerchantItem> Cards { get; set; }

    [JsonPropertyName("rapports")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public List<MerchantItem> Rapports { get; set; }

    [JsonPropertyName("votes")]
    public int Votes { get; set; }
}