using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Handlers
{
    internal class MenuHandlerClass
    {
        public static async Task MenuHandler(SocketMessageComponent component)
        {
            Dictionary<string, string> eventImages = new()
            {
                //Tier 1 Guardian Raids
                { "Ur'nil", "https://i.imgur.com/BrSXgsC.jpg" },
                { "Lumerus", "https://i.imgur.com/tcLXMsq.jpg" },
                { "Icy Legeros", "https://i.imgur.com/p8NY4f3.jpg" },
                { "Vertus", "https://i.imgur.com/69CsRLL.jpg" },
                { "Chromanium", "https://i.imgur.com/dbu8eec.jpg" },
                { "Nacrasena", "https://i.imgur.com/dXaOUIx.jpg" },
                { "Flame Fox Yoho", "https://i.imgur.com/1bWYv9g.jpg" },
                { "Tytalos", "https://i.imgur.com/udA4DLf.jpg" },

                //Tier 2 Guardian Raids
                { "Dark Legoros", "https://i.imgur.com/ug55jBL.jpg" },
                { "Helgia", "https://i.imgur.com/dAkzIpu.jpg" },
                { "Calventus", "https://i.imgur.com/zQY7Zda.jpg" },
                { "Achates", "https://i.imgur.com/C0nME6Q.jpg" },
                { "Frost Helgia", "https://i.imgur.com/U5Z6Vwn.jpg" },
                { "Lava Chromanium", "https://i.imgur.com/frcwlGC.jpg" },
                { "Levanos", "https://i.imgur.com/y0j1ra8.jpg" },
                { "Alberhastic", "https://i.imgur.com/R0s0aJq.jpg" },

                //Tier 3 Guardian Raids
                { "Armored Nacrasena", "https://i.imgur.com/DJ2iEC0.jpg" },
                { "Igrexion", "https://i.imgur.com/nrjzEW6.jpg" },
                { "Night Fox Yoho", "https://i.imgur.com/BGLpgXl.jpg" },
                { "Velganos", "https://i.imgur.com/1ZCbEhk.jpg" },
                { "Deskaluda", "https://i.imgur.com/ICp26Xk.png" },

                //Shushire Abyssal Dungeon
                { "Demon Beast Canyon", "https://i.imgur.com/cPsSgar.jpg" },
                { "Necromancer's Origin", "https://i.imgur.com/Ozymchu.jpg" },

                //Rohendel Abyssal Dungeon
                { "Hall of the Twisted Warlord", "https://i.imgur.com/gFfHrEm.jpg" },
                { "Hildebrandt Palace", "https://i.imgur.com/9VFmKHT.jpg" },

                //Yorn Abyssal Dungeon
                { "Road of Lament", "https://i.imgur.com/fQhnkZQ.jpg" },
                { "Forge of Fallen Pride", "https://i.imgur.com/spaDnDF.jpg" },

                //Feiton Abyssal Dungeon
                { "Sea of Indolence", "https://i.imgur.com/oWmbruI.jpg" },
                { "Tranquil Karkosa", "https://i.imgur.com/toFgbTT.jpg" },
                { "Alaric's Sanctuary", "https://i.imgur.com/6GRZFIP.jpg" },

                //Punika Abyssal Dungeon
                { "Aira's Oculus [Normal]", "https://i.imgur.com/L589GiA.jpg" },
                { "Aira's Oculus [Hard]", "https://i.imgur.com/L589GiA.jpg" },
                { "Oreha Preveza [Normal]", "https://i.imgur.com/IgVX7pu.jpg" },
                { "Oreha Preveza [Hard]", "https://i.imgur.com/IgVX7pu.jpg" },

                //Argos
                { "Argos Phase 1", "https://i.imgur.com/dHak5ox.jpg" },
                { "Argos Phase 2", "https://i.imgur.com/dHak5ox.jpg" },
                { "Argos Phase 3", "https://i.imgur.com/dHak5ox.jpg" },

                //Valtan
                { "Valtan Gate 1 [Normal]", "https://i.imgur.com/lWGHAxj.png" },
                { "Valtan Gate 2 [Normal]", "https://i.imgur.com/lWGHAxj.png" },
                { "Valtan Gate 1 [Hard]", "https://i.imgur.com/lWGHAxj.png" },
                { "Valtan Gate 2 [Hard]", "https://i.imgur.com/lWGHAxj.png" },
                { "Valtan Full [Normal]", "https://i.imgur.com/lWGHAxj.png" },
                { "Valtan Full [Hard]", "https://i.imgur.com/lWGHAxj.png" },

                //Cube
                { "Cube", "https://i.imgur.com/1NmM7St.png" },
                { "Elite Cube", "https://i.imgur.com/dOIurSp.png" },
                { "Dimensional Cube", "https://i.imgur.com/lqkN86W.png" },

                //Boss Rush
                { "Hall of Silence", "https://i.imgur.com/ciwgg3G.png" },
                { "Hall of the Sun", "https://i.imgur.com/PDE70Br.png" },

                //Platinum Fields
                { "Nahun's Domain", "https://i.imgur.com/RIF3Ewf.png" },
                { "Old Yudian Canal", "https://i.imgur.com/OBdcU9e.png" },

                //Chaos Maps
                { "chaosMaps", "https://i.imgur.com/JuMUtQt.png" },

                //Coop Battle
                { "coopbattle", "https://i.imgur.com/tvvlWbS.jpg" }
            };

            List<LfgModel> modelList = new();

            #region Guardian Raids
            LfgModel model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "guardianraid",
                MenuPlaceholder = "Tier of Guardian",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Tier 1", "t1guardianraid"),
                    new MenuBuilderOption("Tier 2", "t2guardianraid"),
                    new MenuBuilderOption("Tier 3", "t3guardianraid"),
                },
                Title = "Guardian Raid",
                Description = "Select the Tier of the Guardian",
                ThumbnailUrl = StaticObjects.guardianIconUrl,
                Color = Color.Red
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "guardianraid" },
                MenuItemId = "t1guardianraid",
                MenuPlaceholder = "Select Guardian",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Ur'nil", "Ur'nil", "Item Level: 302"),
                    new MenuBuilderOption("Lumerus", "Lumerus", "Item Level: 340"),
                    new MenuBuilderOption("Icy Legeros", "Icy Legeros", "Item Level: 380"),
                    new MenuBuilderOption("Vertus", "Vertus", "Item Level: 420"),
                    new MenuBuilderOption("Chromanium", "Chromanium", "Item Level: 460"),
                    new MenuBuilderOption("Nacrasena", "Nacrasena", "Item Level: 500"),
                    new MenuBuilderOption("Flame Fox Yoho", "Flame Fox Yoho", "Item Level: 540"),
                    new MenuBuilderOption("Tytalos", "Tytalos", "Item Level: 580"),
                },
                Title = "Tier 1 Guardian Raid",
                Description = "Select the Guardian you want to do",
                ThumbnailUrl = StaticObjects.guardianIconUrl,
                Color = Color.Red
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "guardianraid" },
                MenuItemId = "t2guardianraid",
                MenuPlaceholder = "Select Guardian",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Dark Legoros", "Dark Legoros", "Item Level: 802"),
                    new MenuBuilderOption("Helgia", "Helgia", "Item Level: 840"),
                    new MenuBuilderOption("Calventus", "Calventus", "Item Level: 880"),
                    new MenuBuilderOption("Achates", "Achates", "Item Level: 920"),
                    new MenuBuilderOption("Frost Helgia", "Frost Helgia", "Item Level: 960"),
                    new MenuBuilderOption("Lava Chromanium", "Lava Chromanium", "Item Level: 1000"),
                    new MenuBuilderOption("Levanos", "Levanos", "Item Level: 1040"),
                    new MenuBuilderOption("Alberhastic", "Alberhastic", "Item Level: 1080"),
                },
                Title = "Tier 2 Guardian Raid",
                Description = "Select the Guardian you want to do",
                ThumbnailUrl = StaticObjects.guardianIconUrl,
                Color = Color.Red
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "guardianraid" },
                MenuItemId = "t3guardianraid",
                MenuPlaceholder = "Select Guardian",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Armored Nacrasena", "Armored Nacrasena", "Item Level: 1302"),
                    new MenuBuilderOption("Igrexion", "Igrexion", "Item Level: 1340"),
                    new MenuBuilderOption("Night Fox Yoho", "Night Fox Yoho", "Item Level: 1370"),
                    new MenuBuilderOption("Velganos", "Velganos", "Item Level: 1385"),
                    new MenuBuilderOption("Deskaluda", "Deskaluda", "Item Level: 1415"),
                },
                Title = "Tier 3 Guardian Raid",
                Description = "Select the Guardian you want to do",
                ThumbnailUrl = StaticObjects.guardianIconUrl,
                Color = Color.Red
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "t1guardianraid", "t2guardianraid", "t3guardianraid" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Guardian Raid] {component.Data.Values.First()} (0/4)",
                ThumbnailUrl = StaticObjects.guardianIconUrl,
                Color = Color.Red,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            #region Abyssal Dungeon
            model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "abyssdungeon",
                MenuPlaceholder = "Select Continent",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Shushire", "shushire", "iLvl: 340"),
                    new MenuBuilderOption("Rohendel", "rohendel", "iLvl: 460"),
                    new MenuBuilderOption("Yorn", "yorn", "iLvl: 840"),
                    new MenuBuilderOption("Feiton", "feiton", "iLvl: 960"),
                    new MenuBuilderOption("Punika", "punika", "iLvl: 1325-1370"),
                },
                Title = "Abyssal Dungeon",
                Description = "Select the Continent of the Dungeon",
                ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                Color = Color.Teal
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "abyssdungeon" },
                MenuItemId = "shushire",
                MenuPlaceholder = "Select Dungeon",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Demon Beast Canyon", "Demon Beast Canyon", "Item Level: 340"),
                    new MenuBuilderOption("Necromancer's Origin", "Necromancer's Origin", "Item Level: 340"),
                },
                Title = "Shushire Abyssal Dungeon",
                Description = "Select the Shushire Abyssal Dungeon you want to do",
                ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "abyssdungeon" },
                MenuItemId = "rohendel",
                MenuPlaceholder = "Select Dungeon",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Hall of the Twisted Warlord", "Hall of the Twisted Warlord", "Item Level: 460"),
                    new MenuBuilderOption("Hildebrandt Palace", "Hildebrandt Palace", "Item Level: 460"),
                },
                Title = "Rohendel Abyssal Dungeon",
                Description = "Select the Rohendel Abyssal Dungeon you want to do",
                ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "abyssdungeon" },
                MenuItemId = "yorn",
                MenuPlaceholder = "Select Dungeon",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Road of Lament", "Road of Lament", "Item Level: 840"),
                    new MenuBuilderOption("Forge of Fallen Pride", "Forge of Fallen Pride", "Item Level: 840"),
                },
                Title = "Yorn Abyssal Dungeon",
                Description = "Select the Yorn Abyssal Dungeon you want to do",
                ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "abyssdungeon" },
                MenuItemId = "feiton",
                MenuPlaceholder = "Select Dungeon",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Sea of Indolence", "Sea of Indolence", "Item Level: 960"),
                    new MenuBuilderOption("Tranquil Karkosa", "Tranquil Karkosa", "Item Level: 960"),
                    new MenuBuilderOption("Alaric's Sanctuary", "Alaric's Sanctuary", "Item Level: 960"),
                },
                Title = "Feiton Abyssal Dungeon",
                Description = "Select the Feiton Abyssal Dungeon you want to do",
                ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "abyssdungeon" },
                MenuItemId = "punika",
                MenuPlaceholder = "Select Dungeon",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Aira's Oculus [Normal]", "Aira's Oculus [Normal]", "Item Level: 1325"),
                    new MenuBuilderOption("Oreha Preveza [Normal]", "Oreha Preveza [Normal]", "Item Level: 1340"),
                    new MenuBuilderOption("Aira's Oculus [Hard]", "Aira's Oculus [Hard]", "Item Level: 1370"),
                    new MenuBuilderOption("Oreha Preveza [Hard]", "Oreha Preveza [Hard]", "Item Level: 1370"),
                },
                Title = "Punika Abyssal Dungeon",
                Description = "Select the Punika Abyssal Dungeon you want to do",
                ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "shushire", "rohendel", "yorn", "punika" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Abyssal Dungeon] {component.Data.Values.First()} (0/4)",
                ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                Color = Color.Teal,
                IsEnd = true,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "feiton" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Abyssal Dungeon] {component.Data.Values.First()} (0/8)",
                ThumbnailUrl = StaticObjects.abyssDungeonIconUrl,
                Color = Color.Teal,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            #region Abyssal Raid
            model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "abyssraid",
                MenuPlaceholder = "Select Abyss Raid",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Argos", "argos", "iLvl: 1370-1400"),
                },
                Title = "Abyssal Raid",
                Description = "Select the Abyssal Raid you want to do",
                ThumbnailUrl = StaticObjects.abyssRaidIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "abyssraid" },
                MenuItemId = "argos",
                MenuPlaceholder = "Select Argos Phase",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Argos Phase 1", "Argos Phase 1", "Item Level: 1370"),
                    new MenuBuilderOption("Argos Phase 2", "Argos Phase 2", "Item Level: 1385"),
                    new MenuBuilderOption("Argos Phase 3", "Argos Phase 3", "Item Level: 1400"),
                },
                Title = "Argos",
                Description = "Select the Argos Phase you want to do",
                ThumbnailUrl = StaticObjects.abyssRaidIconUrl,
                Color = Color.Teal,

            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "argos" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Abyssal Raid] {component.Data.Values.First()} (0/8)",
                ThumbnailUrl = StaticObjects.abyssRaidIconUrl,
                Color = Color.Teal,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            #region Cube
            model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "cube",
                MenuPlaceholder = "Select Tier",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Tier 1", "Cube", "iLvl: 302"),
                    new MenuBuilderOption("Tier 2", "Elite Cube", "iLvl: 802"),
                    new MenuBuilderOption("Tier 3", "Dimensional Cube", "iLvl: 1302"),
                },
                Title = "Cube",
                Description = "Select the Cube Tier you want to do",
                ThumbnailUrl = StaticObjects.cubeIconUrl,
                Color = Color.Red,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "cube" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Cube] {component.Data.Values.First()} (0/4)",
                ThumbnailUrl = StaticObjects.cubeIconUrl,
                Color = Color.Red,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            #region Boss Rush
            model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "bossrush",
                MenuPlaceholder = "Select Tier",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Tier 2", "Hall of Silence", "iLvl: 802"),
                    new MenuBuilderOption("Tier 3", "Hall of the Sun", "iLvl: 1302"),
                },
                Title = "Boss Rush",
                Description = "Select the Boss Rush Tier you want to do",
                ThumbnailUrl = StaticObjects.bossRushIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "bossrush" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Boss Rush] {component.Data.Values.First()} (0/4)",
                ThumbnailUrl = StaticObjects.bossRushIconUrl,
                Color = Color.Teal,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            #region Platinum Fields
            model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "platinumfields",
                MenuPlaceholder = "Select Platinum Field",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Nahun's Domain", "Nahun's Domain", "Foraging/Logging/Mining"),
                    new MenuBuilderOption("Old Yudian Canal", "Old Yudian Canal", "Hunting/Fishing/Excavating"),
                },
                Title = "Platinum Fields",
                Description = "Select which Platinum Field you want to do",
                ThumbnailUrl = StaticObjects.platinumFieldsIconUrl,
                Color = Color.Green,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "platinumfields" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Platinum Fields] {component.Data.Values.First()} (0/4)",
                ThumbnailUrl = StaticObjects.platinumFieldsIconUrl,
                Color = Color.Green,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            #region Chaos Maps
            model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "chaosmaps",
                MenuPlaceholder = "Select Tier",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Tier 1", "Tier 1", "iLvl: 302"),
                    new MenuBuilderOption("Tier 2", "Tier 2", "iLvl: 802"),
                    new MenuBuilderOption("Tier 3", "Tier 3", "iLvl: 1302"),
                },
                Title = "Chaos Maps",
                Description = "Select which Tier of Chaos Maps you want to do",
                ThumbnailUrl = StaticObjects.chaosMapsIconUrl,
                Color = Color.Gold,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "chaosmaps" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Chaos Maps] {component.Data.Values.First()} (0/4)",
                ThumbnailUrl = StaticObjects.chaosMapsIconUrl,
                Color = Color.Gold,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            #region Legion Raid
            model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "legionraid",
                MenuPlaceholder = "Select Legion Commander",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Valtan", "valtan", "iLvl: 1415-1445"),
                },
                Title = "Legion Raid",
                Description = "Select the Legion Commander you want to do",
                ThumbnailUrl = StaticObjects.legionRaidIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "legionraid" },
                MenuItemId = "valtan",
                MenuPlaceholder = "Select Valtan Gate and Difficulty",
                MenuBuilderOptions = new List<MenuBuilderOption>()
                {
                    new MenuBuilderOption("Valtan Full [Normal]", "Valtan Full [Normal]", "Item Level: 1415"),
                    new MenuBuilderOption("Valtan Gate 1 [Normal]", "Valtan Gate 1 [Normal]", "Item Level: 1415"),
                    new MenuBuilderOption("Valtan Gate 2 [Normal]", "Valtan Gate 2 [Normal]", "Item Level: 1415"),
                    new MenuBuilderOption("Valtan Full [Hard]", "Valtan Full [Hard]", "Item Level: 1445"),
                    new MenuBuilderOption("Valtan Gate 1 [Hard]", "Valtan Gate 1 [Hard]", "Item Level: 1445"),
                    new MenuBuilderOption("Valtan Gate 2 [Hard]", "Valtan Gate 2 [Hard]", "Item Level: 1415"),
                },
                Title = "Valtan",
                Description = "Select the Valtan Gate and Difficulty you want to do",
                ThumbnailUrl = StaticObjects.legionRaidIconUrl,
                Color = Color.Teal,
            };
            modelList.Add(model);

            model = new()
            {
                MenuId = new[] { "valtan" },
                MenuItemId = component.Data.Values.First(),
                Title = $"[Legion Raid] {component.Data.Values.First()} (0/8)",
                ThumbnailUrl = StaticObjects.legionRaidIconUrl,
                Color = Color.Teal,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            #region Coop Battle
            model = new()
            {
                MenuId = new[] { "home-lfg" },
                MenuItemId = "coopbattle",
                Title = $"[Coop Battle] Coop Battle (0/6)",
                ThumbnailUrl = StaticObjects.coopBattleIconUrl,
                Color = Color.Red,
                IsEnd = true,
            };
            modelList.Add(model);
            #endregion

            if (modelList.Any(x => x.MenuId.Contains(component.Data.CustomId)))
            {
                LfgModel resultModel = modelList.Find(x => x.MenuId.Contains(component.Data.CustomId) && x.MenuItemId == component.Data.Values.First());

                await LfgHandler.LfgHandlerAsync(component, resultModel, eventImages);
            } else if(component.Data.CustomId == "join")
            {
                throw new NotImplementedException();
            } else if(component.Data.CustomId == "kick")
            {
                throw new NotImplementedException();
            }
        }
    }
}