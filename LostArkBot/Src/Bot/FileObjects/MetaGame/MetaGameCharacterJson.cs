using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.MetaGame;

public class MetaGameCharacterJson
{
    [JsonPropertyName("statsList")]
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Stat> Stats { get; set; }

    [JsonPropertyName("equipList")]
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Equipment> Equipments { get; set; }

    [JsonPropertyName("expeditionLvl")]
    public int RosterLevel { get; set; }

    [JsonPropertyName("pvpRank")]
    public PvpRank PvpRank { get; set; }

    [JsonPropertyName("pcLevel")]
    public int Level { get; set; }

    [JsonPropertyName("guildName")]
    public string GuildName { get; set; }
}