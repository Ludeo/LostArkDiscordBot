using Discord;

namespace LostArkBot.Src.Bot.SlashCommandsOld
{
    internal class RollInitialization
    {
        public static SlashCommandBuilder Roll()
        {
            SlashCommandBuilder rollCommand = new SlashCommandBuilder()
                .WithName("roll")
                .WithDescription("Rolls a random number between 0 and 100 for every user in the lfg");

            return rollCommand;
        }
    }
}