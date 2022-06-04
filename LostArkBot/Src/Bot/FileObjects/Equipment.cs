using System.Text.Json.Serialization;

namespace LostArkBot.Src.Bot.FileObjects
{
    internal class Equipment
    {
        [JsonPropertyName("slotType")]
        public int SlotType { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("itemLevel")]
        public string ItemLevel { get; set; }

        [JsonPropertyName("stats")]
        public EquipmentStat Stats { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        public string Color { get; set; }
    }
}
