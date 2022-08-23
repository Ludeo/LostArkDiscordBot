using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.MetaGame;

// ReSharper disable once ClassNeverInstantiated.Global
public class Stat
{
    [JsonPropertyName("value")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Value { get; set; }

    [JsonPropertyName("description")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Description { get; set; }
}