using System.Text.Json.Serialization;
using LostArkBot.Bot.Shared;

namespace LostArkBot.Bot.FileObjects;

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

    [JsonPropertyName("dbserver")]
    public string DbServer { get; set; }

    [JsonPropertyName("dbname")]
    public string DbName { get; set; }

    [JsonPropertyName("dbuser")]
    public string DbUser { get; set; }

    [JsonPropertyName("dbpassword")]
    public string DbPassword { get; set; }

    [JsonPropertyName("timeOffsetHours")]
    public int TimeOffsetHours { get; set; }

    private static Config GetConfig() => JsonParsers.GetConfigFromJson();
}