using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects.MetaGame
{
    internal class MetaGameRefresh
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("activePcName")]
        public string CharacterName { get; set; }
    }
}
