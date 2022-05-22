using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class EditMessageInitialization
    {
        public static SlashCommandBuilder EditMessage()
        {
            SlashCommandBuilder editMessageCommand = new SlashCommandBuilder()
                                                 .WithName("editmessage")
                                                 .WithDescription("Edits the custom message of an LFG event")
                                                 .AddOption(new SlashCommandOptionBuilder()
                                                            .WithName("message-id")
                                                            .WithDescription("ID of the message (right click message, copy id)")
                                                            .WithRequired(true)
                                                            .WithType(ApplicationCommandOptionType.String))
                                                 .AddOption(new SlashCommandOptionBuilder()
                                                            .WithName("custom-message")
                                                            .WithDescription("New Custom Message for the event")
                                                            .WithRequired(true)
                                                            .WithType(ApplicationCommandOptionType.String));

            return editMessageCommand;
        }
    }
}