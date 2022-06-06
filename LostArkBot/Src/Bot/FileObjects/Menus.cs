using Discord;

namespace LostArkBot.Src.Bot.FileObjects
{
    public static class Menus
    {
        public static SelectMenuBuilder GetHomeLfg()
        {
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                                            .WithPlaceholder("Select event")
                                            .WithCustomId("home-lfg")
                                            .AddOption("Guardian Raid", "guardianraid")
                                            .AddOption("Abyssal Dungeon", "abyssdungeon")
                                            .AddOption("Abyssal Raid", "abyssraid")
                                            .AddOption("Legion Raid", "legionraid")
                                            .AddOption("Cube", "cube")
                                            .AddOption("Boss Rush", "bossrush")
                                            .AddOption("Platinum Fields", "platinumfields")
                                            .AddOption("Chaos Maps", "chaosmaps")
                                            .AddOption("Coop Battle", "coopbattle");

            return menuBuilder;
        }
    }
}
