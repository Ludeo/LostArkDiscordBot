using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.MetaGame
{
    internal class MetaGameCharacterJson
    {
        [JsonPropertyName("statsList")]
        public List<Stat> Stats { get; set; }

        [JsonPropertyName("equipList")]
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
}
