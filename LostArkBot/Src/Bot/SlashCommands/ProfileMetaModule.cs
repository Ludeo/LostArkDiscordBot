using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects.MetaGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    public class ProfileMetaModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("profilemeta", "Shows a picture of the metagame profile of the given character")]
        public async Task ProfileMeta([Summary("character-name", "Name of the character you want to see the profile from")] string characterName)
        {
            string amazonBaseLink = "https://cdn.lostark.games.aws.dev/";

            List<Dictionary<string, Engraving>> engravingListDict = JsonSerializer.Deserialize<List<Dictionary<string, Engraving>>>(File.ReadAllText("engravings.json"));
            Dictionary<string, Engraving> engravingDict = engravingListDict.First();
            List<Engraving> engravings = new();

            foreach (string key in engravingDict.Keys)
            {
                Engraving engraving = engravingDict[key];
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

                engravings.Add(engraving);
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
                await RespondAsync(text: characterName + " does not exist. Login with the character and enable the twitch extension", ephemeral: true);

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

            foreach (Accessory accessory in accessories)
            {
                if (!string.IsNullOrEmpty(accessory.Engraving1))
                {
                    string engravingString = accessory.Engraving1;
                    string engravingName = engravingString[1..engravingString.IndexOf("]")];
                    int engravingValue = int.Parse(engravingString.Split("+")[1]);

                    Engraving engraving = engravings.First(x => x.Name == engravingName);
                    engravings.Remove(engraving);
                    engraving.Value += engravingValue;
                    engravings.Add(engraving);
                }

                if (!string.IsNullOrEmpty(accessory.Engraving2))
                {
                    string engravingString = accessory.Engraving2;
                    string engravingName = engravingString[1..engravingString.IndexOf("]")];
                    int engravingValue = int.Parse(engravingString.Split("+")[1]);

                    Engraving engraving = engravings.First(x => x.Name == engravingName);
                    engravings.Remove(engraving);
                    engraving.Value += engravingValue;
                    engravings.Add(engraving);
                }

                if (!string.IsNullOrEmpty(accessory.BadEngraving))
                {
                    string engravingString = accessory.BadEngraving;
                    string engravingName = engravingString[1..engravingString.IndexOf("]")];
                    int engravingValue = int.Parse(engravingString.Split("+")[1]);

                    Engraving engraving = engravings.First(x => x.Name == engravingName);
                    engravings.Remove(engraving);
                    engraving.Value += engravingValue;
                    engravings.Add(engraving);
                }
            }

            engravings.RemoveAll(x => x.Value < 5);
            List<Engraving> sortedEngravings = engravings.OrderByDescending(x => x.Value).ToList();

            await RespondAsync("Processing..");

            await ProfileScreenShot.MakeProfileScreenshot(sortedEngravings, armorPieces, accessories, metaGameCharacter, metaGameCharacterJson, characterName);

            string path = Environment.CurrentDirectory + "\\image.png";
            
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

            await ModifyOriginalResponseAsync(x => {
                x.Embed = embed.Build();
                x.Attachments = files;
                x.Content = null;
            });
        }
    }
}