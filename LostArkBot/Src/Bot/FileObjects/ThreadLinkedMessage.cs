using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    internal class ThreadLinkedMessage
    {
        [JsonPropertyName("messageid")]
        public ulong MessageId { get; set; }

        [JsonPropertyName("threadid")]
        public ulong ThreadId { get; set; }
    }
}