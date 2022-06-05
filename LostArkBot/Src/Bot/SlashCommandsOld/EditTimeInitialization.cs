using Discord;

namespace LostArkBot.Src.Bot.SlashCommandsOld
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
                                                            .WithDescription("Time of the event in the format DD/MM hh:mm")
                                                            .WithRequired(true)
                                                            .WithType(ApplicationCommandOptionType.String));

            return editTimeCommand;
        }
    }
}
