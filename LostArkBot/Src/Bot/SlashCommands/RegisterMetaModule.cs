using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects.MetaGame;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class RegisterMetaModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("registermeta", "Registers the currently logged in character")]
        public async Task RegisterMeta([Summary("twitch-name", "Your twitch name")] string twitchName)
        {
            await DeferAsync(ephemeral: true);

            string updateUrl = "https://lostark-lookup.herokuapp.com/api/refresh?twitchUsername=" + twitchName;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(updateUrl);
            string responseString = string.Empty;

            try
            {
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using Stream stream = response.GetResponseStream();
                using StreamReader reader = new(stream);
                responseString = reader.ReadToEnd();
            }
            catch (WebException exception)
            {
                HttpWebResponse response = (HttpWebResponse)exception.Response;

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    await FollowupAsync(text: "This user is currently not online or doesn't have the twitch extension enabled");

                    return;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    await FollowupAsync(text: "This twitch user doesn't exist");

                    return;
                }
            }

            MetaGameRefresh metaGameRefresh = JsonSerializer.Deserialize<MetaGameRefresh>(responseString);
            List<Character> characterList = JsonSerializer.Deserialize<List<Character>>(await File.ReadAllTextAsync("characters.json"));
            Character characterCheck = characterList.FirstOrDefault(x => x.CharacterName == metaGameRefresh.CharacterName);

            if (characterCheck != null)
            {
                await FollowupAsync(text: characterCheck.CharacterName + " is already registered. You can update it with **/update**");

                return;
            }

            Character character = new()
            {
                CharacterName = metaGameRefresh.CharacterName,
                CustomProfileMessage = "",
                ProfilePicture = "",
                Engravings = "",
                DiscordUserId = Context.User.Id,
            };

            string queryUrl = "https://lostark-lookup.herokuapp.com/api/query?pcName=" + character.CharacterName;
            request = (HttpWebRequest)WebRequest.Create(queryUrl);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new(stream))
            {
                responseString = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(responseString) || responseString == "[]")
            {
                await FollowupAsync(text: character.CharacterName + " does not exist. Login with the character and enable the twitch extension");

                return;
            }

            if (responseString.StartsWith("["))
            {
                responseString = responseString.Remove(0, 1);
            }
            if (responseString.EndsWith("]"))
            {
                responseString = responseString.Remove(responseString.Length - 1, 1);
            }

            MetaGameCharacter metaGameCharacter = JsonSerializer.Deserialize<MetaGameCharacter>(responseString);
            string characterJson = metaGameCharacter.JsonData.Replace(@"\" + "\"", "\"");
            MetaGameCharacterJson metaGameCharacterJson = JsonSerializer.Deserialize<MetaGameCharacterJson>(characterJson);

            foreach (Stat stat in metaGameCharacterJson.Stats)
            {
                if (stat.Description == "Crit")
                {
                    character.Crit = int.Parse(stat.Value);
                }
                else if (stat.Description == "Specialization")
                {
                    character.Spec = int.Parse(stat.Value);
                }
                else if (stat.Description == "Domination")
                {
                    character.Dom = int.Parse(stat.Value);
                }
                else if (stat.Description == "Swiftness")
                {
                    character.Swift = int.Parse(stat.Value);
                }
                else if (stat.Description == "Endurance")
                {
                    character.End = int.Parse(stat.Value);
                }
                else if (stat.Description == "Expertise")
                {
                    character.Exp = int.Parse(stat.Value);
                }
            }

            character.ItemLevel = (int)metaGameCharacter.ItemLevel;
            character.ClassName = metaGameCharacter.ClassName;
            characterList.Add(character);

            await File.WriteAllTextAsync("characters.json", JsonSerializer.Serialize(characterList));

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {character.CharacterName}",
                ThumbnailUrl = Context.Guild.GetUser(character.DiscordUserId).GetAvatarUrl(),
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", character.ItemLevel, true);
            embedBuilder.AddField("Class", character.ClassName, true);

            embedBuilder.AddField("Engravings", "\u200b", true);
            embedBuilder.AddField("Stats", $"Crit: {character.Crit}\nSpec: {character.Spec}\nDom: {character.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {character.Swift}\nEnd: {character.End}\nExp: {character.Exp}", true);
            //embedBuilder.AddField("Custom Message", "\u200b");

            await FollowupAsync(text: character.CharacterName + " got successfully registered", embed: embedBuilder.Build());
        } 
    }
}