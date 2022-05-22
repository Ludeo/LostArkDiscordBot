using Discord;

namespace LostArkBot.Src.Bot.SlashCommands
{
    internal class StartMerchantInitialization
    {
        public static SlashCommandBuilder StartMerchant()
        {
            SlashCommandBuilder startMerchantCommand = new SlashCommandBuilder()
                                                           .WithName("startmerchant")
                                                           .WithDescription("Starts the merchant timer");

            return startMerchantCommand;
        }
    }
}