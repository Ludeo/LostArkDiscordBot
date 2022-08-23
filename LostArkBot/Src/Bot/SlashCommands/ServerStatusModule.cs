using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace LostArkBot.Bot.SlashCommands;

public class ServerStatusModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    [SlashCommand("serverstatus", "Shows the current status of the Wei server")]
    public async Task ServerStatus()
    {
        await this.DeferAsync();

        string responseJson = await new HttpClient().GetStringAsync("https://lastarkapi-m2.herokuapp.com/server/Wei");
        string status = JsonDocument.Parse(responseJson).RootElement.GetProperty("data").GetProperty("Wei").ToString();

        EmbedBuilder embed = new()
        {
            Title = "Wei Server Status",
            Description = $"``{status}``",
        };

        if (status.EndsWith("Ok"))
        {
            embed.Color = Color.Green;
        }
        else if (status.EndsWith("Busy"))
        {
            embed.Color = Color.Red;
        }
        else if (status.EndsWith("Maintenance"))
        {
            embed.Color = Color.Orange;
        }
        else if (status.EndsWith("Full"))
        {
            embed.Color = Color.DarkRed;
        }

        await this.FollowupAsync(embed: embed.Build());
    }
}