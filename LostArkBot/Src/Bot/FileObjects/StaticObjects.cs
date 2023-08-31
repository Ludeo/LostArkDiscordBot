using System;
using System.Collections.Generic;
using Discord;
using LostArkBot.Bot.Models;

namespace LostArkBot.Bot.FileObjects;

public class StaticObjects
{
    public StaticObjects()
    {
        this.HomeLfgInitialization();
        this.ButtonsInitialization();
        this.EventImagesInitialization();
        this.MenuModelsInitialization();
    }

    public ButtonBuilder DeleteButton { get; private set; }

    public ButtonBuilder HomeButton { get; private set; }

    public ButtonBuilder JoinButton { get; private set; }

    public ButtonBuilder LeaveButton { get; private set; }

    public ButtonBuilder StartButton { get; private set; }

    public ButtonBuilder KickButton { get; private set; }

    public ButtonBuilder ConfirmDeleteButton { get; private set; }

    public ButtonBuilder CancelButton { get; private set; }

    private static string AbyssDungeonIconUrl => "https://i.imgur.com/cjYoBNw.png";

    private static string CoopBattleIconUrl => "https://i.imgur.com/I1zUTq1.png";

    private static string LegionRaidIconUrl => "https://i.imgur.com/IXVEHTH.png";

    public Dictionary<string, string> EventImages { get; private set; }

    public SelectMenuBuilder HomeLfg { get; private set; }

    public List<LfgModel> LfgModels { get; set; } = new();

    public List<ManageUserModel> ManageUserModels { get; } = new();

    public static TimeSpan TimeOffset { get; set; } = new(Config.Default.TimeOffsetHours, 0, 0);

    private void ButtonsInitialization()
    {
        this.DeleteButton = new ButtonBuilder().WithCustomId("deletebutton").WithLabel("Delete").WithStyle(ButtonStyle.Danger);
        this.HomeButton = new ButtonBuilder().WithCustomId("homebutton").WithLabel("Home").WithStyle(ButtonStyle.Primary);
        this.JoinButton = new ButtonBuilder().WithCustomId("joinbutton").WithLabel("Join").WithStyle(ButtonStyle.Primary);
        this.LeaveButton = new ButtonBuilder().WithCustomId("leavebutton").WithLabel("Leave").WithStyle(ButtonStyle.Danger);
        this.StartButton = new ButtonBuilder().WithCustomId("startbutton").WithLabel("Start").WithStyle(ButtonStyle.Secondary);
        this.KickButton = new ButtonBuilder().WithCustomId("kickbutton").WithLabel("Kick").WithStyle(ButtonStyle.Danger);
        this.ConfirmDeleteButton = new ButtonBuilder().WithCustomId("confirmdeletebutton").WithLabel("Confirm").WithStyle(ButtonStyle.Danger);
        this.CancelButton = new ButtonBuilder().WithCustomId("cancelbutton").WithLabel("Cancel").WithStyle(ButtonStyle.Secondary);
    }

    private void EventImagesInitialization() =>
        this.EventImages = new Dictionary<string, string>
        {
            //Kayangel
            { "Kayangel [Normal]", "https://i.imgur.com/YqVZxzl.jpg" },
            { "Kayangel [Hard]", "https://i.imgur.com/YqVZxzl.jpg" },

            //Valtan
            { "Valtan Full [Normal]", "https://i.imgur.com/lWGHAxj.png" },
            { "Valtan Full [Hard]", "https://i.imgur.com/lWGHAxj.png" },

            //Vykas
            { "Vykas Full [Normal]", "https://c.tenor.com/pTau_jrQlcEAAAAC/lost-ark-biakiss.gif" },
            { "Vykas Full [Hard]", "https://c.tenor.com/pTau_jrQlcEAAAAC/lost-ark-biakiss.gif" },

            //Kakul Saydon
            { "Kakul Saydon Full [Normal]", "https://i.imgur.com/8bkMCGJ.jpg" },

            //Brelshaza
            { "Brelshaza 1-2 [Normal]", "https://i.imgur.com/H7jeO1i.jpg" },
            { "Brelshaza 1-4 [Normal]", "https://i.imgur.com/H7jeO1i.jpg" },
            { "Brelshaza 1-6 [Normal]", "https://i.imgur.com/H7jeO1i.jpg" },
            { "Brelshaza 1-2 [Hard]", "https://i.imgur.com/H7jeO1i.jpg" },
            { "Brelshaza 1-4 [Hard]", "https://i.imgur.com/H7jeO1i.jpg" },
            { "Brelshaza 1-6 [Hard]", "https://i.imgur.com/H7jeO1i.jpg" },

            //Akkan
            { "Akkan 1-3 [Normal]", "https://i.imgur.com/3nVKDLh.jpg" },
            { "Akkan 1-3 [Hard]", "https://i.imgur.com/3nVKDLh.jpg" },

            //Guild Siege
            { "Snowpang Island", "https://assets.maxroll.gg/wordpress/Islands-island_snowpang_2.jpg" },
            { "Naruni Island", "https://assets.maxroll.gg/wordpress/Islands-island_naruni.jpg" },
            { "Death's Hold Island", "https://assets.maxroll.gg/wordpress/Islands-island_deaths_hold.jpg" },
            { "Tranquil Isle", "https://assets.maxroll.gg/wordpress/Islands-island_tranquil.jpg" },
            { "Slime Island", "https://assets.maxroll.gg/wordpress/Islands-island_drumbeat.jpg" },
            { "Golden Wave", "https://assets.maxroll.gg/wordpress/Islands-island_golden_wave.jpg" },
            { "Lush Reed", "https://assets.maxroll.gg/wordpress/Islands-island_lush_reed.jpg" },
            { "Medeia", "https://assets.maxroll.gg/wordpress/Islands-island_medeia.jpg" },
        };

    private void HomeLfgInitialization()
    {
        SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                                        .WithPlaceholder("Select event")
                                        .WithCustomId("home-lfg")
                                        .AddOption("Abyssal Dungeon", "abyssdungeon")
                                        .AddOption("Legion Raid", "legionraid")
                                        .AddOption("Guild Siege", "guildsiege");

        this.HomeLfg = menuBuilder;
    }

    private void MenuModelsInitialization()
    {
        #region Abyssal Dungeon

        LfgModel model = new LfgModel
        {
            MenuId = new[] { "home-lfg" },
            MenuItemId = "abyssdungeon",
            MenuPlaceholder = "Select Continent",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Elgacia", "elgacia", "iLvl: 1540-1580"),
            },
            Title = "Abyssal Dungeon",
            Description = "Select the Continent of the Dungeon",
            ThumbnailUrl = AbyssDungeonIconUrl,
            Color = Color.Teal,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "abyssdungeon" },
            MenuItemId = "elgacia",
            MenuPlaceholder = "Select Dungeon",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Kayangel [Normal]", "Kayangel [Normal]", "Item Level: 1540"),
                new("Kayangel [Hard]", "Kayangel [Hard]", "Item Level: 1580"),
            },
            Title = "Punika Abyssal Dungeon",
            Description = "Select the Punika Abyssal Dungeon you want to do",
            ThumbnailUrl = AbyssDungeonIconUrl,
            Color = Color.Teal,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "elgacia" },
            Title = "[Abyssal Dungeon]",
            ThumbnailUrl = AbyssDungeonIconUrl,
            Color = Color.Teal,
            IsEnd = true,
            Players = 4,
        };

        this.LfgModels.Add(model);

        #endregion

        #region Legion Raid

        model = new LfgModel
        {
            MenuId = new[] { "home-lfg" },
            MenuItemId = "legionraid",
            MenuPlaceholder = "Select Legion Commander",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Valtan", "valtan", "iLvl: 1415-1445"),
                new("Vykas", "vykas", "iLvl: 1430-1460"),
                new("Kakul Saydon", "kakulsaydon", "iLvl: 1475"),
                new("Brelshaza", "brelshaza", "iLvl: 1490-1520"),
                new("Akkan", "akkan", "iLvl: 1580-1600"),
            },
            Title = "Legion Raid",
            Description = "Select the Legion Commander you want to do",
            ThumbnailUrl = LegionRaidIconUrl,
            Color = Color.Teal,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "legionraid" },
            MenuItemId = "valtan",
            MenuPlaceholder = "Select Valtan Gate and Difficulty",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Valtan Full [Normal]", "Valtan Full [Normal]", "Item Level: 1415"),
                new("Valtan Full [Hard]", "Valtan Full [Hard]", "Item Level: 1445"),
            },
            Title = "Valtan",
            Description = "Select the Valtan Gate and Difficulty you want to do",
            ThumbnailUrl = LegionRaidIconUrl,
            Color = Color.Teal,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "legionraid" },
            MenuItemId = "vykas",
            MenuPlaceholder = "Select Vykas Gate and Difficulty",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Vykas Full [Normal]", "Vykas Full [Normal]", "Item Level: 1430"),
                new("Vykas Full [Hard]", "Vykas Full [Hard]", "Item Level: 1460"),
            },
            Title = "Vykas",
            Description = "Select the Vykas Gate and Difficulty you want to do",
            ThumbnailUrl = LegionRaidIconUrl,
            Color = Color.Teal,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "legionraid" },
            MenuItemId = "kakulsaydon",
            MenuPlaceholder = "Select Kakul Saydon Gate and Difficulty",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Kakul Saydon Full [Normal]", "Kakul Saydon Full [Normal]", "Item Level: 1475"),
            },
            Title = "Kakul Saydon",
            Description = "Select the Kakul Saydon Gate and Difficulty you want to do",
            ThumbnailUrl = LegionRaidIconUrl,
            Color = Color.Teal,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "legionraid" },
            MenuItemId = "brelshaza",
            MenuPlaceholder = "Select Brelshaza Gate and Difficulty",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Brelshaza 1-2 [Normal]", "Brelshaza 1-2 [Normal]", "Item Level: 1490"),
                new("Brelshaza 1-4 [Normal]", "Brelshaza 1-4 [Normal]", "Item Level: 1500"),
                new("Brelshaza 1-6 [Normal]", "Brelshaza 1-6 [Normal]", "Item Level: 1520"),
                new("Brelshaza 1-2 [Hard]", "Brelshaza 1-2 [Hard]", "Item Level: 1540"),
                new("Brelshaza 1-4 [Hard]", "Brelshaza 1-4 [Hard]", "Item Level: 1550"),
                new("Brelshaza 1-6 [Hard]", "Brelshaza 1-6 [Hard]", "Item Level: 1560"),
            },
            Title = "Brelshaza",
            Description = "Select the Brelshaza Gate and Difficulty you want to do",
            ThumbnailUrl = LegionRaidIconUrl,
            Color = Color.Teal,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "legionraid" },
            MenuItemId = "akkan",
            MenuPlaceholder = "Select Akkan Gate and Difficulty",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Akkan 1-3 [Normal]", "Akkan 1-3 [Normal]", "Item Level: 1580"),
                new("Akkan 1-3 [Hard]", "Akkan 1-3 [Hard]", "Item Level: 1600"),
            },
            Title = "Akkan",
            Description = "Select the Akkan Gate and Difficulty you want to do",
            ThumbnailUrl = LegionRaidIconUrl,
            Color = Color.Teal,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "valtan", "vykas", "brelshaza", "akkan" },
            Title = "[Legion Raid]",
            ThumbnailUrl = LegionRaidIconUrl,
            Color = Color.Teal,
            IsEnd = true,
            Players = 8,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "kakulsaydon" },
            Title = "[Legion Raid]",
            ThumbnailUrl = LegionRaidIconUrl,
            Color = Color.Teal,
            IsEnd = true,
            Players = 4,
        };

        this.LfgModels.Add(model);

        #endregion

        #region Siege

        model = new LfgModel
        {
            MenuId = new[] { "home-lfg" },
            MenuItemId = "guildsiege",
            MenuPlaceholder = "Select Siege",
            MenuBuilderOptions = new List<MenuBuilderOption>
            {
                new("Naruni Island", "Naruni Island", "8 people"),
                new("Snowpang Island", "Snowpang Island", "8 people"),
                new("Death's Hold Island", "Death's Hold Island", "12 people"),
                new("Tranquil Isle", "Tranquil Isle", "16 people"),
                new("Slime Island", "Slime Island", "23 people"),
                new("Golden Wave", "Golden Wave", "8 people"),
                new("Lush Reed", "Lush Reed", "12 people"),
                new("Medeia", "Medeia", "23 people"),
            },
            Title = "Guild Siege",
            Description = "Select the Guild Siege you want to do",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "guildsiege" },
            MenuItemId = "Naruni Island",
            Title = "[Guild Siege]",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
            IsEnd = true,
            Players = 8,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "guildsiege" },
            MenuItemId = "Snowpang Island",
            Title = "[Guild Siege]",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
            IsEnd = true,
            Players = 8,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "guildsiege" },
            MenuItemId = "Death's Hold Island",
            Title = "[Guild Siege]",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
            IsEnd = true,
            Players = 12,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "guildsiege" },
            MenuItemId = "Tranquil Isle",
            Title = "[Guild Siege]",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
            IsEnd = true,
            Players = 16,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "guildsiege" },
            MenuItemId = "Slime Island",
            Title = "[Guild Siege]",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
            IsEnd = true,
            Players = 23,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "guildsiege" },
            MenuItemId = "Golden Wave",
            Title = "[Guild Siege]",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
            IsEnd = true,
            Players = 8,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "guildsiege" },
            MenuItemId = "Lush Reed",
            Title = "[Guild Siege]",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
            IsEnd = true,
            Players = 12,
        };

        this.LfgModels.Add(model);

        model = new LfgModel
        {
            MenuId = new[] { "guildsiege" },
            MenuItemId = "Medeia",
            Title = "[Guild Siege]",
            ThumbnailUrl = CoopBattleIconUrl,
            Color = Color.Red,
            IsEnd = true,
            Players = 23,
        };

        this.LfgModels.Add(model);

        #endregion

        #region Manage Users

        ManageUserModel manageUserModel = new()
        {
            MenuId = "join",
            Action = ManageAction.Join,
        };

        this.ManageUserModels.Add(manageUserModel);

        manageUserModel = new ManageUserModel
        {
            MenuId = "kick",
            Action = ManageAction.Kick,
        };

        this.ManageUserModels.Add(manageUserModel);

        #endregion
    }
}