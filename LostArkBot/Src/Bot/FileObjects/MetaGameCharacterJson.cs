using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    internal class MetaGameCharacterJson
    {
        [JsonPropertyName("statsList")]
        public List<Stat> Stats { get; set; } 
    }
}
