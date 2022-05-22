using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class HelpInitialization
    {
        public static SlashCommandBuilder Help()
        {
            SlashCommandBuilder helpCommand = new SlashCommandBuilder()
                .WithName("help")
                .WithDescription("Shows all the commands");

            return helpCommand;
        }
    }
}