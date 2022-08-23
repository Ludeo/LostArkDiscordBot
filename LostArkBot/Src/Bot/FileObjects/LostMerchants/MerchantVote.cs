using System.Text.Json.Serialization;

namespace LostArkBot.Bot.FileObjects.LostMerchants;

public class MerchantVote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("votes")]
    public int Votes { get; set; }
}