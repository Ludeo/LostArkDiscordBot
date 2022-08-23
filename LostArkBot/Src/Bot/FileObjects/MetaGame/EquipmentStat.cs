using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.MetaGame;

// ReSharper disable once ClassNeverInstantiated.Global
public class EquipmentStat
{
    [JsonPropertyName("Random Engraving Effect")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<string> Engravings { get; set; }

    [JsonPropertyName("Bonus Effect")]
    // ReSharper disable once CollectionNeverUpdated.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public List<string> BonusEffect { get; set; }
}