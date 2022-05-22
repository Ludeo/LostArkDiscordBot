using Discord;

namespace LostArkBot.Src.Bot.FileObjects
{
    public static class StaticObjects
    {
        public static ButtonBuilder deleteButton = new ButtonBuilder().WithCustomId("delete").WithLabel("Delete").WithStyle(ButtonStyle.Danger);
        public static ButtonBuilder homeButton = new ButtonBuilder().WithCustomId("home").WithLabel("Home").WithStyle(ButtonStyle.Primary);
        public static ButtonBuilder joinButton = new ButtonBuilder().WithCustomId("join").WithLabel("Join").WithStyle(ButtonStyle.Primary);
        public static ButtonBuilder leaveButton = new ButtonBuilder().WithCustomId("leave").WithLabel("Leave").WithStyle(ButtonStyle.Danger);
        public static ButtonBuilder startButton = new ButtonBuilder().WithCustomId("start").WithLabel("Start").WithStyle(ButtonStyle.Secondary);
        public static ButtonBuilder kickButton = new ButtonBuilder().WithCustomId("kick").WithLabel("Kick").WithStyle(ButtonStyle.Danger);

        public static string guardianIconUrl = "https://i.imgur.com/ZN7spLT.png";
        public static string abyssDungeonIconUrl = "https://i.imgur.com/cjYoBNw.png";
        public static string cubeIconUrl = "https://i.imgur.com/uqifrZh.png";
        public static string bossRushIconUrl = "https://i.imgur.com/yQn3Sx7.png";
        public static string platinumFieldsIconUrl = "https://i.imgur.com/Ivy5r0M.png";
        public static string chaosMapsIconUrl = "https://i.imgur.com/nXG6UQF.png";
        public static string coopBattleIconUrl = "https://i.imgur.com/I1zUTq1.png";
        public static string abyssRaidIconUrl = "https://i.imgur.com/OTFz9Le.png";
        public static string legionRaidIconUrl = "https://i.imgur.com/IXVEHTH.png";
    }
}