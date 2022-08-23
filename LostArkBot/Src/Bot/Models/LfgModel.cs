using System.Collections.Generic;
using Discord;

namespace LostArkBot.Bot.Models;

public class LfgModel
{
    public string[] MenuId { get; init; }

    public string MenuItemId { get; init; }

    public string MenuPlaceholder { get; init; }

    public List<MenuBuilderOption> MenuBuilderOptions { get; set; } = new();

    public string Title { get; init; }

    public string Description { get; init; }

    public Color Color { get; init; }

    public string ThumbnailUrl { get; init; }

    public bool IsEnd { get; init; }

    public int Players { get; init; }
}