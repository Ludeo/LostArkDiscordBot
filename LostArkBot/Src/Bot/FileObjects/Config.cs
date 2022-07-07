using LostArkBot.Src.Bot.Shared;
using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    /// <summary>
    ///     Class that represents the config file for the Bot.
    /// </summary>
    public class Config
    {
        /// <summary>
        ///     Gets the Default Config object.
        /// </summary>
        public static Config Default { get; } = GetConfig();

        /// <summary>
        ///     Gets or sets the discord bot token.
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        ///     Gets or sets the discord id of the lost ark server.
        /// </summary>
        [JsonPropertyName("server")]
        public ulong Server { get; set; }

        [JsonPropertyName("admin")]
        public ulong Admin { get; set; }

        [JsonPropertyName("merchantchannel")]
        public ulong MerchantChannel { get; set; }

        private static Config GetConfig()
        {
            return JsonParsers.GetConfigFromJson();
        }
    }
}