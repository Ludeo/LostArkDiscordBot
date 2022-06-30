using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.MetaGame
{
    public class EquipmentStat
    {
        [JsonPropertyName("Random Engraving Effect")]
        public List<string> Engravings { get; set; }

        [JsonPropertyName("Bonus Effect")]
        public List<string> BonusEffect { get; set; }
    }
}