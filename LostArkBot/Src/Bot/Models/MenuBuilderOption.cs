namespace LostArkBot.Bot.Models;

public class MenuBuilderOption
{
    public MenuBuilderOption(string label, string value, string description = null)
    {
        this.Label = label;
        this.Value = value;
        this.Description = description;
    }

    public string Value { get; }

    public string Label { get; }

    public string Description { get; }
}