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
                                            .AddOption("Event Guardian Raid", "eventguardianraid")
                                            .AddOption("Coop Battle", "coopbattle");

            return menuBuilder;
        }

        public static SelectMenuBuilder GetGuardianRaidTier()
        {
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                                            .WithPlaceholder("Tier of Guardian")
                                            .WithCustomId("guardianraidtier")
                                            .AddOption("Tier 1", "t1guardianraid")
                                            .AddOption("Tier 2", "t2guardianraid")
                                            .AddOption("Tier 3", "t3guardianraid");

            return menuBuilder;
        }
    }
}
