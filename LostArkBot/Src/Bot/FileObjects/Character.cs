using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects
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
        public string Crit { get; set; }

        [JsonPropertyName("spec")]
        public string Spec { get; set; }

        [JsonPropertyName("dom")]
        public string Dom { get; set; }

        [JsonPropertyName("swift")]
        public string Swift { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }

        [JsonPropertyName("exp")]
        public string Exp { get; set; }

        [JsonPropertyName("profile-picture")]
        public string ProfilePicture { get; set; }

        [JsonPropertyName("custom-profile-message")]
        public string CustomProfileMessage { get; set; }
    }
}