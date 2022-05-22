using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class RollInitialization
    {
        public static SlashCommandBuilder Roll()
        {
            SlashCommandBuilder rollCommand = new SlashCommandBuilder()
                .WithName("roll")
                .WithDescription("Rolls a random number between 0 and 100 for every user in the lfg")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("message-id")
                    .WithDescription("Message ID of the LFG event")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String));

            return rollCommand;
        }
    }
}