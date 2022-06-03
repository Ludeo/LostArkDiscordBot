using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class EditTimeInitialization
    {
        public static SlashCommandBuilder EditTime()
        {
            SlashCommandBuilder editTimeCommand = new SlashCommandBuilder()
                                                 .WithName("edittime")
                                                 .WithDescription("Edits the time of an LFG event")
                                                 .AddOption(new SlashCommandOptionBuilder()
                                                            .WithName("time")
                                                            .WithDescription("Time of the event in the format MM/DD hh:mm")
                                                            .WithRequired(true)
                                                            .WithType(ApplicationCommandOptionType.String));

            return editTimeCommand;
        }
    }
}
