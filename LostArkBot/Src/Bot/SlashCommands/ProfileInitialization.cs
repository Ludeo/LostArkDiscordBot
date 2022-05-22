using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class ProfileInitialization
    {
        public static SlashCommandBuilder Profile()
        {
            SlashCommandBuilder profileCommand = new SlashCommandBuilder()
                                                     .WithName("profile")
                                                     .WithDescription("Shows the profile of the character")
                                                     .AddOption(new SlashCommandOptionBuilder()
                                                                .WithName("character-name")
                                                                .WithDescription("Name of the character")
                                                                .WithRequired(true)
                                                                .WithType(ApplicationCommandOptionType.String));

            return profileCommand;
        }
    }
}