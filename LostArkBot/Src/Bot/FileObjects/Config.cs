using System.IO;
using System.Text.Json;
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
        ///     Gets or sets the discord bot prefix.
        /// </summary>
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; } = "b!";

        /// <summary>
        ///     Gets or sets the discord id of the bot admin.
        /// </summary>
        [JsonPropertyName("adminid")]
        public ulong AdminId { get; set; }

        /// <summary>
        ///     Gets or sets the discord id of the channel where the merchant times get send in.
        /// </summary>
        [JsonPropertyName("merchantchannel")]
        public ulong MerchantChannel { get; set; }

        [JsonPropertyName("lfgchannel")]
        public ulong LfgChannel { get; set; }

        /// <summary>
        ///     Gets or sets the discord id of the lost ark server.
        /// </summary>
        [JsonPropertyName("server")]
        public ulong Server { get; set; }

        /// <summary>
        ///     Gets or sets the unix time of the last checked merchant 1.
        /// </summary>
        [JsonPropertyName("merchant1")]
        public long Merchant1 { get; set; }

        /// <summary>
        ///     Gets or sets the unix time of the last checked merchant 2.
        /// </summary>
        [JsonPropertyName("merchant2")]
        public long Merchant2 { get; set; }

        /// <summary>
        ///     Gets or sets the unix time of the last checked merchant 3.
        /// </summary>
        [JsonPropertyName("merchant3")]
        public long Merchant3 { get; set; }

        private static Config GetConfig()
        {
            if (!File.Exists("config.json"))
            {
                Config configFile = new();
                string jsonData = JsonSerializer.Serialize(configFile);
                File.WriteAllText("config.json", jsonData);

                return configFile;
            }

            return JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
        }
    }
}