using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class ServerStatusModule
    {
        public static async Task ServerStatusAsync(SocketSlashCommand command)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://lostarkapi.herokuapp.com/server/Wei");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string responseJson = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

            dynamic json = JsonConvert.DeserializeObject(responseJson);

            string status = json.data.Wei;

            EmbedBuilder embed = new EmbedBuilder()
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

            await command.RespondAsync(embed: embed.Build());
        }
    }
}