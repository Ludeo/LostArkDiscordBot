using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class LfgInitialization
    {
        public static SlashCommandBuilder Lfg()
        {
            SlashCommandBuilder lfgCommand = new SlashCommandBuilder()
                                                 .WithName("lfg")
                                                 .WithDescription("Creates a LFG event")
                                                 .AddOption(new SlashCommandOptionBuilder()
                                                            .WithName("custom-message")
                                                            .WithDescription("Custom Message for the event")
                                                            .WithRequired(false)
                                                            .WithType(ApplicationCommandOptionType.String))
                                                 .AddOption(new SlashCommandOptionBuilder()
                                                            .WithName("time")
                                                            .WithDescription("Time for the event")
                                                            .WithRequired(false)
                                                            .WithType(ApplicationCommandOptionType.String));

            return lfgCommand;
        }
    }
}