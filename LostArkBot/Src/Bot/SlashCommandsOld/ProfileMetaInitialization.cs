using Discord;

namespace LostArkBot.Src.Bot.SlashCommandsOld
{
    internal class ProfileMetaInitialization
    {
        public static SlashCommandBuilder ProfileMeta()
        {
            SlashCommandBuilder profileMetaCommand = new SlashCommandBuilder()
                                                 .WithName("profilemeta")
                                                 .WithDescription("Shows your profile from the meta-game website")
                                                 .AddOption(new SlashCommandOptionBuilder()
                                                            .WithName("character-name")
                                                            .WithDescription("Name of the character")
                                                            .WithRequired(true)
                                                            .WithType(ApplicationCommandOptionType.String));

            return profileMetaCommand;
        }
    }
}
