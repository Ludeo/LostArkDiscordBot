﻿using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class RegisterInitialization
    {
        public static SlashCommandBuilder Register()
        {
            SlashCommandBuilder registerCommand = new SlashCommandBuilder()
                                                      .WithName("register")
                                                      .WithDescription("Registers a character to your account")
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("character-name")
                                                                 .WithDescription("Name of your character, caps sensitive!")
                                                                 .WithRequired(true)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("class-name")
                                                                 .WithDescription("Class of your character")
                                                                 .WithRequired(true)
                                                                 .AddChoice("Artillerist", "Artillerist")
                                                                 .AddChoice("Bard", "Bard")
                                                                 .AddChoice("Berserker", "Berserker")
                                                                 .AddChoice("Deadeye", "Deadeye")
                                                                 .AddChoice("Deathblade", "Deathblade")
                                                                 .AddChoice("Destroyer", "Destroyer")
                                                                 .AddChoice("Glaivier", "Glaivier")
                                                                 .AddChoice("Gunlancer", "Gunlancer")
                                                                 .AddChoice("Gunslinger", "Gunslinger")
                                                                 .AddChoice("Paladin", "Paladin")
                                                                 .AddChoice("Scrapper", "Scrapper")
                                                                 .AddChoice("Shadowhunter", "Shadowhunter")
                                                                 .AddChoice("Sharpshooter", "Sharpshooter")
                                                                 .AddChoice("Sorceress", "Sorceress")
                                                                 .AddChoice("Soulfist", "Soulfist")
                                                                 .AddChoice("Striker", "Striker")
                                                                 .AddChoice("Wardancer", "Wardancer")
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("item-level")
                                                                 .WithDescription("Item Level of your character, no decimals")
                                                                 .WithRequired(true)
                                                                 .WithType(ApplicationCommandOptionType.Integer))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("engravings")
                                                                 .WithDescription("Engravings of the character SEPARATED BY A COMMA")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("crit")
                                                                 .WithDescription("Critical stat of the character")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("spec")
                                                                 .WithDescription("Specialization stat of the character")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("dom")
                                                                 .WithDescription("Domination stat of the character")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("swift")
                                                                 .WithDescription("Swiftness stat of the character")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("end")
                                                                 .WithDescription("Endurance stat of the character")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("exp")
                                                                 .WithDescription("Expertise stat of the character")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("profile-picture")
                                                                 .WithDescription("Link for profile picture")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String))
                                                      .AddOption(new SlashCommandOptionBuilder()
                                                                 .WithName("custom-profile-message")
                                                                 .WithDescription("Custom message that gets displayed on your profile")
                                                                 .WithRequired(false)
                                                                 .WithType(ApplicationCommandOptionType.String));
            return registerCommand;
        }
    }
}