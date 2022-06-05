using Discord;

namespace LostArkBot.Src.Bot.SlashCommandsOld
{
    internal class DeleteInitialization
    {
        public static SlashCommandBuilder Delete()
        {
            SlashCommandBuilder deleteCommand = new SlashCommandBuilder()
                                                     .WithName("delete")
                                                     .WithDescription("Deletes the character")
                                                     .AddOption(new SlashCommandOptionBuilder()
                                                                .WithName("character-name")
                                                                .WithDescription("Name of the character")
                                                                .WithRequired(true)
                                                                .WithType(ApplicationCommandOptionType.String));

            return deleteCommand;
        }
    }
}