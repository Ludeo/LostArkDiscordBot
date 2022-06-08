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
        ///     Gets or sets the discord id of the lost ark server.
        /// </summary>
        [JsonPropertyName("server")]
        public ulong Server { get; set; }

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