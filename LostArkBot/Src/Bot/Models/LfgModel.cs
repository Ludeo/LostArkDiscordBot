using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Models
{
    public class LfgModel
    {
        public string[] MenuId { get; set; }

        public string MenuItemId { get; set; }

        public string MenuPlaceholder { get; set; }

        public List<MenuBuilderOption> MenuBuilderOptions { get; set; } = new();

        public string Title { get; set; }

        public string Description { get; set; }

        public Color Color { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ImageUrl { get; set; }

        public bool IsEnd { get; set; } = false;
    }
}
