using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace LostArkBot.Bot.Modules
{
    public class PingMerchantRolesModule : ModuleBase<SocketCommandContext>
    {
        public async Task StartPingMerchantRolesAsync(SocketMessage rawMessage)
        {
            if (rawMessage.Content.Contains("@Seria"))
            {
                await rawMessage.Channel.SendMessageAsync("<@&953946761066602506>");
            }

            if (rawMessage.Content.Contains("@Sian card"))
            {
                await rawMessage.Channel.SendMessageAsync("<@&953946814439104542>");
            }

            if (rawMessage.Content.Contains("@Madnick"))
            {
                await rawMessage.Channel.SendMessageAsync("<@&953946889148071936>");
            }

            if (rawMessage.Content.Contains("@Wei Card"))
            {
                await rawMessage.Channel.SendMessageAsync("<@&953946987714199582>");
            }

            if (rawMessage.Content.Contains("@Mokamoka"))
            {
                await rawMessage.Channel.SendMessageAsync("<@&953946979514327060>");
            }

            if (rawMessage.Content.Contains("@Legendary Affinity"))
            {
                await rawMessage.Channel.SendMessageAsync("<@&953947894984097802>");
            }
        }
    }
}