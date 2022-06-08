using Discord;

namespace LostArkBot.Src.Bot.FileObjects
{
    public static class StaticObjects
    {
        public static readonly ButtonBuilder deleteButton = new ButtonBuilder().WithCustomId("deletebutton").WithLabel("Delete").WithStyle(ButtonStyle.Danger);
        public static readonly ButtonBuilder homeButton = new ButtonBuilder().WithCustomId("homebutton").WithLabel("Home").WithStyle(ButtonStyle.Primary);
        public static readonly ButtonBuilder joinButton = new ButtonBuilder().WithCustomId("joinbutton").WithLabel("Join").WithStyle(ButtonStyle.Primary);
        public static readonly ButtonBuilder leaveButton = new ButtonBuilder().WithCustomId("leavebutton").WithLabel("Leave").WithStyle(ButtonStyle.Danger);
        public static readonly ButtonBuilder startButton = new ButtonBuilder().WithCustomId("startbutton").WithLabel("Start").WithStyle(ButtonStyle.Secondary);
        public static readonly ButtonBuilder kickButton = new ButtonBuilder().WithCustomId("kickbutton").WithLabel("Kick").WithStyle(ButtonStyle.Danger);

        public static readonly string guardianIconUrl = "https://i.imgur.com/ZN7spLT.png";
        public static readonly string abyssDungeonIconUrl = "https://i.imgur.com/cjYoBNw.png";
        public static readonly string cubeIconUrl = "https://i.imgur.com/uqifrZh.png";
        public static readonly string bossRushIconUrl = "https://i.imgur.com/yQn3Sx7.png";
        public static readonly string platinumFieldsIconUrl = "https://i.imgur.com/Ivy5r0M.png";
        public static readonly string chaosMapsIconUrl = "https://i.imgur.com/nXG6UQF.png";
        public static readonly string coopBattleIconUrl = "https://i.imgur.com/I1zUTq1.png";
        public static readonly string abyssRaidIconUrl = "https://i.imgur.com/OTFz9Le.png";
        public static readonly string legionRaidIconUrl = "https://i.imgur.com/IXVEHTH.png";
    }
}