using Discord;

namespace LostArkBot.Src.Bot.SlashCommandsOld
{
    internal class EditMessageInitialization
    {
        public static SlashCommandBuilder EditMessage()
        {
            SlashCommandBuilder editMessageCommand = new SlashCommandBuilder()
                                                 .WithName("editmessage")
                                                 .WithDescription("Edits the custom message of an LFG event")
                                                 .AddOption(new SlashCommandOptionBuilder()
                                                            .WithName("custom-message")
                                                            .WithDescription("New Custom Message for the event")
                                                            .WithRequired(true)
                                                            .WithType(ApplicationCommandOptionType.String));

            return editMessageCommand;
        }
    }
}