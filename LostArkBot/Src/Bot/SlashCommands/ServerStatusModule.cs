using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class ServerStatusModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("serverstatus", "Shows the current status of the Wei server")]
        public async Task ServerStatus()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://lastarkapi-m2.herokuapp.com/server/Wei");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new(receiveStream, Encoding.UTF8);
            string responseJson = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

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

            await RespondAsync(embed: embed.Build());
        }
    }
}