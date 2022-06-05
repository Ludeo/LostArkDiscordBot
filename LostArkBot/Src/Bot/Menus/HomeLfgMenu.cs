using Discord;

namespace LostArkBot.Src.Bot.Menus
{
    public class HomeLfgMenu
    {
        public static SelectMenuBuilder GetMenu()
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
                                            .AddOption("Event Guardian Raid", "eventguardianraid")
                                            .AddOption("Coop Battle", "coopbattle");

            return menuBuilder;
        }
    }
}
