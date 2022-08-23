using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.databasemodels;
using LostArkBot.Src.Bot.FileObjects.MetaGame;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [Group("meta", "Commands for using the meta-game website for characters")]
    public class MetaModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly LostArkBotContext dbcontext;

        public MetaModule(LostArkBotContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }   

        [SlashCommand("register", "Registers the currently logged in character")]
        public async Task Register([Summary("twitch-name", "Your twitch name")] string twitchName)
        {
            await DeferAsync();

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
                    IMessage message = await FollowupAsync("auto-delete");
                    await message.DeleteAsync();
                    await FollowupAsync(text: "This user is currently not online or doesn't have the twitch extension enabled", ephemeral: true) ;

                    return;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    IMessage message = await FollowupAsync("auto-delete");
                    await message.DeleteAsync();
                    await FollowupAsync(text: "This twitch user doesn't exist", ephemeral: true);

                    return;
                }
            }

            MetaGameRefresh metaGameRefresh = JsonSerializer.Deserialize<MetaGameRefresh>(responseString);
            Character characterCheck = dbcontext.Characters.Where(x => x.CharacterName == metaGameRefresh.CharacterName).FirstOrDefault();

            if (characterCheck != null)
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: characterCheck.CharacterName + " is already registered. You can update it with **/characters update**");

                return;
            }

            User user = dbcontext.Users.Where(x => x.DiscordUserId == Context.User.Id).FirstOrDefault();

            if(user is null)
            {
                EntityEntry<User> userEntry = dbcontext.Users.Add(new User
                {
                    DiscordUserId = Context.User.Id,
                });

                await dbcontext.SaveChangesAsync();
                user = userEntry.Entity;
            }

            Character character = new()
            {
                CharacterName = metaGameRefresh.CharacterName,
                CustomProfileMessage = "",
                ProfilePicture = "",
                Engravings = "",
                UserId = user.Id,
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
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: character.CharacterName + " does not exist. Login with the character and enable the twitch extension", ephemeral: true);

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
            
            dbcontext.Characters.Add(character);
            await dbcontext.SaveChangesAsync();

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {character.CharacterName}",
                ThumbnailUrl = Context.Guild.GetUser(character.User.DiscordUserId).GetAvatarUrl(),
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", character.ItemLevel, true);
            embedBuilder.AddField("Class", character.ClassName, true);

            embedBuilder.AddField("Engravings", "\u200b", true);
            embedBuilder.AddField("Stats", $"Crit: {character.Crit}\nSpec: {character.Spec}\nDom: {character.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {character.Swift}\nEnd: {character.End}\nExp: {character.Exp}", true);
            embedBuilder.AddField("Custom Message", "\u200b");

            await FollowupAsync(text: character.CharacterName + " got successfully registered", embed: embedBuilder.Build());
        }

        [SlashCommand("update", "Updates the profile of the currently logged in character")]
        public async Task Update([Summary("twitch-name", "Your twitch name")] string twitchName)
        {
            await DeferAsync();

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
                    IMessage message = await FollowupAsync("auto-delete");
                    await message.DeleteAsync();
                    await FollowupAsync(text: "This user is currently not online or doesn't have the twitch extension enabled", ephemeral: true);

                    return;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    IMessage message = await FollowupAsync("auto-delete");
                    await message.DeleteAsync();
                    await FollowupAsync(text: "This twitch user doesn't exist", ephemeral: true);

                    return;
                }
            }

            MetaGameRefresh metaGameRefresh = JsonSerializer.Deserialize<MetaGameRefresh>(responseString);

            Character character = dbcontext.Characters.Where(x => x.CharacterName == metaGameRefresh.CharacterName).Include(x => x.User).FirstOrDefault();

            if (character is null)
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: metaGameRefresh.CharacterName + " doesn't exist. You can add it with **/characters add**");

                return;
            }

            if (character.User.DiscordUserId != Context.User.Id)
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: metaGameRefresh.CharacterName + " does not belong to you", ephemeral: true);

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

            if (string.IsNullOrEmpty(responseString) || responseString == "[]")
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: character.CharacterName + " does not exist. Login with the character and enable the twitch extension", ephemeral: true);

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

            dbcontext.Characters.Update(character);
            await dbcontext.SaveChangesAsync();

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {character.CharacterName}",
                ThumbnailUrl = character.ProfilePicture == string.Empty
                    ? Context.Guild.GetUser(character.User.DiscordUserId).GetAvatarUrl()
                    : character.ProfilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", character.ItemLevel, true);
            embedBuilder.AddField("Class", character.ClassName, true);

            string engraving = string.Empty;

            if (!string.IsNullOrEmpty(character.Engravings))
            {
                string[] engravings = character.Engravings.Split(",");

                foreach (string x in engravings)
                {
                    engraving += x + "\n";
                }
            }

            embedBuilder.AddField("Engravings", engraving == string.Empty ? "\u200b" : engraving, true);
            embedBuilder.AddField("Stats", $"Crit: {character.Crit}\nSpec: {character.Spec}\nDom: {character.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {character.Swift}\nEnd: {character.End}\nExp: {character.Exp}", true);

            if (character.CustomProfileMessage != string.Empty)
            {
                embedBuilder.AddField("Custom Message", character.CustomProfileMessage);
            }

            await FollowupAsync(text: $"{character.CharacterName} got successfully updated", embed: embedBuilder.Build());
        }

        [SlashCommand("profile", "Shows a picture of the metagame profile of the given character")]
        public async Task Profile([Summary("character-name", "Name of the character you want to see the profile from")] string characterName)
        {
            await DeferAsync();

            string amazonBaseLink = "https://cdn.lostark.games.aws.dev/";
            List<Engraving> engravings = dbcontext.Engravings.ToList();

            foreach (Engraving engraving in engravings)
            {
                string[] icon = engraving.Icon.Split("_");

                if (icon.Length > 0 && !string.IsNullOrEmpty(icon[0]))
                {
                    if (icon[0] == "GL")
                    {
                        engraving.Icon = amazonBaseLink + "EFUI_IconAtlas/GL_Skill/" + engraving.Icon + ".png";
                    }
                    else if (icon[0] == "achieve")
                    {
                        engraving.Icon = amazonBaseLink + "EFUI_IconAtlas/Achieve/" + engraving.Icon + ".png";
                    }
                    else
                    {
                        engraving.Icon = amazonBaseLink + "EFUI_IconAtlas/" + icon[0] + "/" + engraving.Icon + ".png";
                    }
                }
            }

            string queryUrl = "https://lostark-lookup.herokuapp.com/api/query?pcName=" + characterName;
            WebRequest request = (HttpWebRequest)WebRequest.Create(queryUrl);
            string responseString = string.Empty;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new(stream))
            {
                responseString = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(responseString) || responseString == "[]")
            {
                IMessage message = await FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await FollowupAsync(text: characterName + " does not exist. Login with the character and enable the twitch extension", ephemeral: true);

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

            List<ArmorPiece> armorPieces = new();

            List<Accessory> accessories = new();

            foreach (Equipment equipment in metaGameCharacterJson.Equipments)
            {
                if (equipment.Type == "Legendary")
                {
                    equipment.Color = "#ad6500";
                }
                else if (equipment.Type == "Relic")
                {
                    equipment.Color = "#a84203";
                }
                else if (equipment.Type == "Rare")
                {
                    equipment.Color = "#123f60";
                }
                else if (equipment.Type == "Epic")
                {
                    equipment.Color = "#470d5a";
                }
                else if (equipment.Type == "Uncommon")
                {
                    equipment.Color = "#118a1d";
                }
                else if (equipment.Type == "Common")
                {
                    equipment.Color = "#666666";
                }

                switch (equipment.SlotType)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        armorPieces.Add(new()
                        {
                            Name = equipment.Name,
                            ItemLevel = equipment.ItemLevel,
                            Icon = amazonBaseLink + equipment.Icon,
                            Color = equipment.Color,
                        });

                        break;
                    case 6:
                        accessories.Add(new()
                        {
                            Name = equipment.Name,
                            Stat1 = equipment.Stats.BonusEffect[0],
                            Stat2 = equipment.Stats.BonusEffect[1],
                            Engraving1 = equipment.Stats.Engravings[0],
                            Engraving2 = equipment.Stats.Engravings[1],
                            BadEngraving = equipment.Stats.Engravings[2],
                            Icon = amazonBaseLink + equipment.Icon,
                            Color = equipment.Color,
                        });

                        break;
                    case 7:
                    case 8:
                        accessories.Add(new()
                        {
                            Name = equipment.Name,
                            Stat1 = equipment.Stats.BonusEffect[0],
                            Engraving1 = equipment.Stats.Engravings[0],
                            Engraving2 = equipment.Stats.Engravings[1],
                            BadEngraving = equipment.Stats.Engravings[2],
                            Icon = amazonBaseLink + equipment.Icon,
                            Color = equipment.Color,
                        });

                        break;
                    case 9:
                    case 10:
                        accessories.Add(new()
                        {
                            Name = equipment.Name,
                            Stat1 = equipment.Stats.BonusEffect[0],
                            Engraving1 = equipment.Stats.Engravings[0],
                            Engraving2 = equipment.Stats.Engravings[1],
                            BadEngraving = equipment.Stats.Engravings[2],
                            Icon = amazonBaseLink + equipment.Icon,
                            Color = equipment.Color,
                        });

                        break;
                    case 11:
                        accessories.Add(new()
                        {
                            Name = equipment.Name,
                            Engraving1 = equipment.Stats.Engravings[0],
                            Engraving2 = equipment.Stats.Engravings[1],
                            BadEngraving = equipment.Stats.Engravings[2],
                            Icon = amazonBaseLink + equipment.Icon,
                            Color = equipment.Color,
                        });

                        break;
                    default:
                        break;
                }
            }

            List<MetaEngraving> engravingWithValues = new();

            foreach (Accessory accessory in accessories)
            {
                if (!string.IsNullOrEmpty(accessory.Engraving1))
                {
                    string engravingString = accessory.Engraving1;
                    string engravingName = engravingString[1..engravingString.IndexOf("]")];
                    int engravingValue = int.Parse(engravingString.Split("+")[1]);

                    MetaEngraving engraving = engravingWithValues.FirstOrDefault(x => x.Name == engravingName);

                    if(engraving is null)
                    {
                        engraving = JsonSerializer.Deserialize<MetaEngraving>(JsonSerializer.Serialize(engravings.First(x => x.Name == engravingName)));
                        engraving.Value = 0;
                    } else
                    {
                        engravingWithValues.Remove(engraving);
                    }

                    engraving.Value += engravingValue;
                    engravingWithValues.Add(engraving);
                }

                if (!string.IsNullOrEmpty(accessory.Engraving2))
                {
                    string engravingString = accessory.Engraving2;
                    string engravingName = engravingString[1..engravingString.IndexOf("]")];
                    int engravingValue = int.Parse(engravingString.Split("+")[1]);

                    MetaEngraving engraving = engravingWithValues.FirstOrDefault(x => x.Name == engravingName);

                    if (engraving is null)
                    {
                        engraving = JsonSerializer.Deserialize<MetaEngraving>(JsonSerializer.Serialize(engravings.First(x => x.Name == engravingName)));
                        engraving.Value = 0;
                    }
                    else
                    {
                        engravingWithValues.Remove(engraving);
                    }

                    engraving.Value += engravingValue;
                    engravingWithValues.Add(engraving);
                }

                if (!string.IsNullOrEmpty(accessory.BadEngraving))
                {
                    string engravingString = accessory.BadEngraving;
                    string engravingName = engravingString[1..engravingString.IndexOf("]")];
                    int engravingValue = int.Parse(engravingString.Split("+")[1]);

                    MetaEngraving engraving = engravingWithValues.FirstOrDefault(x => x.Name == engravingName);

                    if (engraving is null)
                    {
                        engraving = JsonSerializer.Deserialize<MetaEngraving>(JsonSerializer.Serialize(engravings.First(x => x.Name == engravingName)));
                        engraving.Value = 0;
                    }
                    else
                    {
                        engravingWithValues.Remove(engraving);
                    }

                    engraving.Value += engravingValue;
                    engravingWithValues.Add(engraving);
                }
            }

            engravingWithValues.RemoveAll(x => x.Value < 5);
            List<MetaEngraving> sortedEngravings = engravingWithValues.OrderByDescending(x => x.Value).ToList();

            await ProfileScreenShot.MakeProfileScreenshot(sortedEngravings, armorPieces, accessories, metaGameCharacter, metaGameCharacterJson, characterName);

            string path = Environment.CurrentDirectory + "/image.png";

            EmbedBuilder embed = new()
            {
                Title = "Meta Game profile of " + characterName,
                Description = "Profile Link: https://lostark.meta-game.gg/armory?character=" + characterName,
                ImageUrl = $"attachment://{Path.GetFileName(path)}",
                Color = Color.Blue
            };

            List<FileAttachment> files = new()
            {
                new FileAttachment(path)
            };

            await FollowupWithFilesAsync(embed: embed.Build(), attachments: files);
        }
    }
}