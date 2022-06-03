using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class WhenInitialization
    {
        public static SlashCommandBuilder When()
        {
            SlashCommandBuilder whenCommand = new SlashCommandBuilder()
                                                 .WithName("when")
                                                 .WithDescription("Shows the time of the lfg (if set)");

            return whenCommand;
        }
    }
}
