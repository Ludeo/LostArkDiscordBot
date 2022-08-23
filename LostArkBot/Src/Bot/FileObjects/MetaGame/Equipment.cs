using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.MetaGame;

// ReSharper disable once ClassNeverInstantiated.Global
public class Equipment
{
    [JsonPropertyName("slotType")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int SlotType { get; set; }

    [JsonPropertyName("icon")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Icon { get; set; }

    [JsonPropertyName("name")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Name { get; set; }

    [JsonPropertyName("itemLevel")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string ItemLevel { get; set; }

    [JsonPropertyName("stats")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public EquipmentStat Stats { get; set; }

    [JsonPropertyName("type")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Type { get; set; }

    public string Color { get; set; }
}