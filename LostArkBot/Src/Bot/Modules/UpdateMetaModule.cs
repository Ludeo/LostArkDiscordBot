using Discord;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.Modules
{
    internal class UpdateMetaModule
    {
        public static async Task UpdateMetaAsync(SocketSlashCommand command)
        {
            string twitchName = command.Data.Options.First(x => x.Name == "twitch-name").Value.ToString();
            string updateUrl = "https://lostark-lookup.herokuapp.com/api/refresh?twitchUsername=" + twitchName;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(updateUrl);
            string responseString = string.Empty;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new(stream))
                {
                    responseString = reader.ReadToEnd();
                }
            } catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;

                if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    await command.RespondAsync(text: "This user is currently not online or doesn't have the twitch extension enabled", ephemeral: true);

                    return;
                } else if(response.StatusCode == HttpStatusCode.BadRequest)
                {
                    await command.RespondAsync(text: "This twitch user doesn't exist", ephemeral: true);

                    return;
                }
            }

            MetaGameRefresh metaGameRefresh = JsonSerializer.Deserialize<MetaGameRefresh>(responseString);

            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));

            Character oldCharacter = characterList.First(x => x.CharacterName == metaGameRefresh.CharacterName);
            Character newCharacter = oldCharacter;

            if(oldCharacter is null)
            {
                await command.RespondAsync(text: metaGameRefresh.CharacterName + " is not registered yet. You can register it with /register", ephemeral: true);
                
                return;
            }

            if(oldCharacter.DiscordUserId != command.User.Id)
            {
                await command.RespondAsync(text: metaGameRefresh.CharacterName + " does not belong to you", ephemeral: true);

                return;
            }

            string queryUrl = "https://lostark-lookup.herokuapp.com/api/query?pcName=" + metaGameRefresh.CharacterName;
            request = (HttpWebRequest)WebRequest.Create(queryUrl);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new(stream))
            {
                responseString = reader.ReadToEnd();
            }

            if(string.IsNullOrEmpty(responseString) || responseString == "[]")
            {
                await command.RespondAsync(text: oldCharacter.CharacterName + " does not exist. Login with the character and enable the twitch extension", ephemeral: true);

                return;
            }

            if(responseString.StartsWith("["))
            {
                responseString = responseString.Remove(0, 1);
            }
            if(responseString.EndsWith("]"))
            {
                responseString = responseString.Remove(responseString.Length - 1, 1);
            }

            MetaGameCharacter metaGameCharacter = JsonSerializer.Deserialize<MetaGameCharacter>(responseString);
            string characterJson = metaGameCharacter.JsonData.Replace(@"\" + "\"", "\"");

            MetaGameCharacterJson metaGameCharacterJson = JsonSerializer.Deserialize<MetaGameCharacterJson>(characterJson);
            foreach(Stat stat in metaGameCharacterJson.Stats)
            {
                if(stat.Description == "Crit")
                {
                    newCharacter.Crit = stat.Value;
                } else if(stat.Description == "Specialization")
                {
                    newCharacter.Spec = stat.Value;
                } else if(stat.Description == "Domination")
                {
                    newCharacter.Dom = stat.Value;
                } else if(stat.Description == "Swiftness")
                {
                    newCharacter.Swift = stat.Value;
                } else if(stat.Description == "Endurance")
                {
                    newCharacter.End = stat.Value;
                } else if(stat.Description == "Expertise")
                {
                    newCharacter.Exp = stat.Value;
                }
            }

            newCharacter.ItemLevel = (int)metaGameCharacter.ItemLevel;

            characterList.Add(newCharacter);
            characterList.Remove(oldCharacter);

            await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {newCharacter.CharacterName}",
                ThumbnailUrl = newCharacter.ProfilePicture == string.Empty
                    ? Program.Client.GetUser(newCharacter.DiscordUserId).GetAvatarUrl()
                    : newCharacter.ProfilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", newCharacter.ItemLevel, true);
            embedBuilder.AddField("Class", newCharacter.ClassName, true);

            string[] engravings = newCharacter.Engravings.Split(",");
            string engraving = string.Empty;

            foreach (string x in engravings)
            {
                engraving += x + "\n";
            }

            embedBuilder.AddField("Engravings", engraving, true);
            embedBuilder.AddField("Stats", $"Crit: {newCharacter.Crit}\nSpec: {newCharacter.Spec}\nDom: {newCharacter.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {newCharacter.Swift}\nEnd: {newCharacter.End}\nExp: {newCharacter.Exp}", true);
            embedBuilder.AddField("Custom Message", newCharacter.CustomProfileMessage == string.Empty ? "\u200b" : newCharacter.CustomProfileMessage);

            await command.RespondAsync(text: $"{newCharacter.CharacterName} got successfully updated", embed: embedBuilder.Build());
        }
    }
}
