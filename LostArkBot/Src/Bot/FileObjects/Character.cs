using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    public class Character
    {
        [JsonPropertyName("discord-user-id")]
        public ulong DiscordUserId { get; set; }

        [JsonPropertyName("character-name")]
        public string CharacterName { get; set; }

        [JsonPropertyName("class-name")]
        public string ClassName { get; set; }

        [JsonPropertyName("item-level")]
        public int ItemLevel { get; set; }

        [JsonPropertyName("engravings")]
        public string Engravings { get; set; }

        [JsonPropertyName("crit")]
        public int Crit { get; set; }

        [JsonPropertyName("spec")]
        public int Spec { get; set; }

        [JsonPropertyName("dom")]
        public int Dom { get; set; }

        [JsonPropertyName("swift")]
        public int Swift { get; set; }

        [JsonPropertyName("end")]
        public int End { get; set; }

        [JsonPropertyName("exp")]
        public int Exp { get; set; }

        [JsonPropertyName("profile-picture")]
        public string ProfilePicture { get; set; }

        [JsonPropertyName("custom-profile-message")]
        public string CustomProfileMessage { get; set; }
    }
}