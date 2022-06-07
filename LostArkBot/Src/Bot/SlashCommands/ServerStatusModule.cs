using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class ServerStatusModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("serverstatus", "Shows the current status of the Wei server")]
        public async Task ServerStatus()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://lostarkapi.herokuapp.com/server/Wei");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new(receiveStream, Encoding.UTF8);
            string responseJson = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

            dynamic json = JsonConvert.DeserializeObject(responseJson);

            string status = json.data.Wei;

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