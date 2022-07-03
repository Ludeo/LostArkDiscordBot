using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    public class StaticGroup
    {
        [JsonPropertyName("leaderid")]
        public ulong LeaderId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("players")]
        public List<string> Players { get; set; }
    }
}