namespace LostArkBot.Src.Bot.Models
{
    public class MenuBuilderOption
    {
        public string Value { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public MenuBuilderOption(string label, string value, string description = null)
        {
            Label = label;
            Value = value;
            Description = description;
        }
    }
}