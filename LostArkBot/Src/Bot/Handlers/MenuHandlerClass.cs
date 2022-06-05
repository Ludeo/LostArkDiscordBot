using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.MenusOld;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    internal class MenuHandlerClass
    {
        public static async Task MenuHandler(SocketMessageComponent component)
        {
            Dictionary<string, string> eventImages = new();
            //Tier 1 Guardian Raids
            eventImages.Add("Ur'nil", "https://i.imgur.com/BrSXgsC.jpg");
            eventImages.Add("Lumerus", "https://i.imgur.com/tcLXMsq.jpg");
            eventImages.Add("Icy Legeros", "https://i.imgur.com/p8NY4f3.jpg");
            eventImages.Add("Vertus", "https://i.imgur.com/69CsRLL.jpg");
            eventImages.Add("Chromanium", "https://i.imgur.com/dbu8eec.jpg");
            eventImages.Add("Nacrasena", "https://i.imgur.com/dXaOUIx.jpg");
            eventImages.Add("Flame Fox Yoho", "https://i.imgur.com/1bWYv9g.jpg");
            eventImages.Add("Tytalos", "https://i.imgur.com/udA4DLf.jpg");
            //Tier 2 Guardian Raids
            eventImages.Add("Dark Legoros", "https://i.imgur.com/ug55jBL.jpg");
            eventImages.Add("Helgia", "https://i.imgur.com/dAkzIpu.jpg");
            eventImages.Add("Calventus", "https://i.imgur.com/zQY7Zda.jpg");
            eventImages.Add("Achates", "https://i.imgur.com/C0nME6Q.jpg");
            eventImages.Add("Frost Helgia", "https://i.imgur.com/U5Z6Vwn.jpg");
            eventImages.Add("Lava Chromanium", "https://i.imgur.com/frcwlGC.jpg");
            eventImages.Add("Levanos", "https://i.imgur.com/y0j1ra8.jpg");
            eventImages.Add("Alberhastic", "https://i.imgur.com/R0s0aJq.jpg");

            //Tier 3 Guardian Raids
            eventImages.Add("Armored Nacrasena", "https://i.imgur.com/DJ2iEC0.jpg");
            eventImages.Add("Igrexion", "https://i.imgur.com/nrjzEW6.jpg");
            eventImages.Add("Night Fox Yoho", "https://i.imgur.com/BGLpgXl.jpg");
            eventImages.Add("Velganos", "https://i.imgur.com/1ZCbEhk.jpg");
            eventImages.Add("Deskaluda", "https://i.imgur.com/ICp26Xk.png");

            //Shushire Abyssal Dungeon
            eventImages.Add("Demon Beast Canyon", "https://i.imgur.com/cPsSgar.jpg");
            eventImages.Add("Necromancer's Origin", "https://i.imgur.com/Ozymchu.jpg");

            //Rohendel Abyssal Dungeon
            eventImages.Add("Hall of the Twisted Warlord", "https://i.imgur.com/gFfHrEm.jpg");
            eventImages.Add("Hildebrandt Palace", "https://i.imgur.com/9VFmKHT.jpg");

            //Yorn Abyssal Dungeon
            eventImages.Add("Road of Lament", "https://i.imgur.com/fQhnkZQ.jpg");
            eventImages.Add("Forge of Fallen Pride", "https://i.imgur.com/spaDnDF.jpg");

            //Feiton Abyssal Dungeon
            eventImages.Add("Sea of Indolence", "https://i.imgur.com/oWmbruI.jpg");
            eventImages.Add("Tranquil Karkosa", "https://i.imgur.com/toFgbTT.jpg");
            eventImages.Add("Alaric's Sanctuary", "https://i.imgur.com/6GRZFIP.jpg");

            //Punika Abyssal Dungeon
            eventImages.Add("Aira's Oculus [Normal]", "https://i.imgur.com/L589GiA.jpg");
            eventImages.Add("Aira's Oculus [Hard]", "https://i.imgur.com/L589GiA.jpg");
            eventImages.Add("Oreha Preveza [Normal]", "https://i.imgur.com/IgVX7pu.jpg");
            eventImages.Add("Oreha Preveza [Hard]", "https://i.imgur.com/IgVX7pu.jpg");

            //Argos
            eventImages.Add("Argos Phase 1", "https://i.imgur.com/dHak5ox.jpg");
            eventImages.Add("Argos Phase 2", "https://i.imgur.com/dHak5ox.jpg");
            eventImages.Add("Argos Phase 3", "https://i.imgur.com/dHak5ox.jpg");

            //Valtan
            eventImages.Add("Valtan Gate 1 [Normal]", "https://i.imgur.com/lWGHAxj.png");
            eventImages.Add("Valtan Gate 2 [Normal]", "https://i.imgur.com/lWGHAxj.png");
            eventImages.Add("Valtan Gate 1 [Hard]", "https://i.imgur.com/lWGHAxj.png");
            eventImages.Add("Valtan Gate 2 [Hard]", "https://i.imgur.com/lWGHAxj.png");
            eventImages.Add("Valtan Full [Normal]", "https://i.imgur.com/lWGHAxj.png");
            eventImages.Add("Valtan Full [Hard]", "https://i.imgur.com/lWGHAxj.png");

            //Cube
            eventImages.Add("Cube", "https://i.imgur.com/1NmM7St.png");
            eventImages.Add("Elite Cube", "https://i.imgur.com/dOIurSp.png");
            eventImages.Add("Dimensional Cube", "https://i.imgur.com/lqkN86W.png");

            //Boss Rush
            eventImages.Add("Hall of Silence", "https://i.imgur.com/ciwgg3G.png");
            eventImages.Add("Hall of the Sun", "https://i.imgur.com/PDE70Br.png");

            //Platinum Fields
            eventImages.Add("Nahun's Domain", "https://i.imgur.com/RIF3Ewf.png");
            eventImages.Add("Old Yudian Canal", "https://i.imgur.com/OBdcU9e.png");

            //Chaos Maps
            eventImages.Add("chaosMaps", "https://i.imgur.com/JuMUtQt.png");

            //Event Guardian Raid
            eventImages.Add("eventGuardianRaid", "https://i.imgur.com/a9L1Q3Z.png");

            //Coop Battle
            eventImages.Add("coopBattle", "https://i.imgur.com/tvvlWbS.jpg");

            switch (component.Data.CustomId)
            {
                case "home-lfg":
                    await HomeLfgMenu.HomeLfg(component, eventImages);

                    break;

                case "guardianraid":
                    await GuardianRaidMenu.GuardianRaid(component, eventImages);

                    break;

                case "t1guardianraid":
                case "t2guardianraid":
                case "t3guardianraid":
                    await GuardianRaidEndMenu.GuardianRaidEnd(component, eventImages);

                    break;

                case "abyssdungeon":
                    await AbyssalDungeonMenu.AbyssalDungeon(component, eventImages);

                    break;

                case "shushiredungeon":
                case "rohendeldungeon":
                case "yorndungeon":
                case "feitondungeon":
                case "punikadungeon":
                    await AbyssalDungeonEndMenu.AbyssalDungeonEnd(component, eventImages);

                    break;

                case "abyssraid":
                    await AbyssalRaidMenu.AbyssalRaid(component, eventImages);
                    break;

                case "argos":
                    await AbyssalRaidEndMenu.AbyssalRaidEnd(component, eventImages);

                    break;

                case "cube":
                    await CubeEndMenu.CubeEnd(component, eventImages);

                    break;

                case "bossrush":
                    await BossRushEndMenu.BossRushEnd(component, eventImages);

                    break;

                case "platinumfields":
                    await PlatinumFieldsEndMenu.PlatinumFieldsEnd(component, eventImages);

                    break;

                case "chaosmaps":
                    await ChaosMapsEndMenu.ChaosMapsEnd(component, eventImages);

                    break;

                case "join":
                    await JoinCharacterMenu.JoinCharacter(component);

                    break;

                case "kick":
                    await KickCharacterMenu.KickCharacter(component);

                    break;

                case "legionraid":
                    await LegionRaidMenu.LegionRaid(component, eventImages);
                    break;

                case "valtan":
                    await LegionRaidEndMenu.LegionRaidEnd(component, eventImages);

                    break;
            }
        }
    }
}