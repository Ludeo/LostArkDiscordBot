using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [Group("characters", "Commands to manage your characters")]
    public class CharactersModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("list", "Shows all of your registered characters")]
        public async Task List()
        {
            ulong userId = Context.User.Id;
            List<Character> characterList = await JsonParsers.GetCharactersFromJsonAsync();
            List<Character> characters = characterList.FindAll(x => x.DiscordUserId == userId);

            if (characters.Count == 0)
            {
                await RespondAsync(text: "You don't have any characters registered. You can register a character with **/register**", ephemeral: true);

                return;
            }

            EmbedBuilder embed = new()
            {
                Title = "Your characters",
                Color = Color.DarkPurple,
                Description = "\u200b",
                ThumbnailUrl = Context.User.GetAvatarUrl(),
            };

            List<GuildEmote> emotes = Program.GuildEmotes;

            foreach (Character character in characters)
            {
                GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

                embed.AddField(new EmbedFieldBuilder()
                {
                    Name = character.CharacterName,
                    Value = $"<:{emote.Name}:{emote.Id}> {character.ClassName}\n{character.ItemLevel}",
                    IsInline = true,
                });
            }

            await RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        [SlashCommand("add", "Adds a new character to your character list")]
        public async Task Add(
            [Summary("character-name", "Name of the character you want to register")] string characterName,
            [Summary("class-name", "Class of your character")] ClassName className,
            [Summary("item-level", "Item Level of your character")] int itemLevel,
            [Summary("engravings", "Engravings of your character, seperated by a comma")] string engravings = "",
            [Summary("crit", "How much Crit your character has")] int crit = 0,
            [Summary("spec", "How much Specialization your character has")] int spec = 0,
            [Summary("dom", "How much Domination your character has")] int dom = 0,
            [Summary("swift", "How much Swiftness your character has")] int swift = 0,
            [Summary("end", "How much Endurance your character has")] int end = 0,
            [Summary("exp", "How much Expertise your character has")] int exp = 0,
            [Summary("profile-picture", "Link for a profile picture")] string profilePicture = "",
            [Summary("custom-profile-message", "Custom message for your profile")] string customMessage = "")
        {
            characterName = characterName.ToTitleCase();
            List<Character> characterList = await JsonParsers.GetCharactersFromJsonAsync();
            Character oldCharacter = characterList.Find(x => x.CharacterName.ToLower() == characterName.ToLower());

            if (oldCharacter is not null)
            {
                await RespondAsync(text: $"{characterName} is already added. You can update it with **/update**", ephemeral: true);
                return;
            }

            Character newCharacter = new()
            {
                DiscordUserId = Context.User.Id,
                CharacterName = characterName,
                ClassName = className.ToString(),
                ItemLevel = itemLevel,
                Engravings = engravings,
                Crit = crit,
                Spec = spec,
                Dom = dom,
                Swift = swift,
                End = end,
                Exp = exp,
                CustomProfileMessage = customMessage,
                ProfilePicture = profilePicture,
            };

            if (!string.IsNullOrEmpty(newCharacter.Engravings))
            {
                newCharacter.Engravings = Utils.ParseEngravings(engravings);
            }

            characterList.Add(newCharacter);
            await JsonParsers.WriteCharactersAsync(characterList);

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = profilePicture == string.Empty ? Context.Guild.GetUser(newCharacter.DiscordUserId).GetAvatarUrl() : profilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", itemLevel, true);
            embedBuilder.AddField("Class", className, true);

            string engravingsString = "\u200b";
            foreach (string x in engravings.Split(","))
            {
                engravingsString += x + "\n";
            }

            embedBuilder.AddField("Engravings", engravingsString, true);
            embedBuilder.AddField("Stats", $"Crit: {crit}\nSpec: {spec}\nDom: {dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {swift}\nEnd: {end}\nExp: {exp}", true);

            if (customMessage != string.Empty)
            {
                embedBuilder.AddField("Custom Message", customMessage);
            }

            await RespondAsync(text: $"{characterName} got successfully registered", embed: embedBuilder.Build());
        }

        [SlashCommand("update", "Updates the given character")]
        public async Task Update(
            [Summary("character-name", "Name of the character you want to register")] string characterName,
            [Summary("class-name", "Class of your character")] ClassName className = ClassName.Default,
            [Summary("item-level", "Item Level of your character")] int itemLevel = 0,
            [Summary("engravings", "Engravings of your character, seperated by a comma")] string engravings = null,
            [Summary("crit", "How much Crit your character has")] int crit = -1,
            [Summary("spec", "How much Specialization your character has")] int spec = -1,
            [Summary("dom", "How much Domination your character has")] int dom = -1,
            [Summary("swift", "How much Swiftness your character has")] int swift = -1,
            [Summary("end", "How much Endurance your character has")] int end = -1,
            [Summary("exp", "How much Expertise your character has")] int exp = -1,
            [Summary("profile-picture", "Link for a profile picture")] string profilePicture = "",
            [Summary("custom-profile-message", "Custom message for your profile")] string customMessage = "")
        {
            List<Character> characterList = await JsonParsers.GetCharactersFromJsonAsync();
            ulong discordUserId = Context.User.Id;
            characterName = characterName.ToTitleCase();

            Character oldCharacter = characterList.Find(x => x.CharacterName.ToLower() == characterName.ToLower());
            Character newCharacter = oldCharacter;

            if (oldCharacter is not null)
            {
                if (oldCharacter.DiscordUserId != discordUserId)
                {
                    await RespondAsync(text: $"You don't have permissions to update {characterName}", ephemeral: true);

                    return;
                }
            }
            else
            {
                await RespondAsync(text: $"{characterName} is not registered. You can register it with **/register**", ephemeral: true);

                return;
            }

            if (className is not ClassName.Default)
            {
                newCharacter.ClassName = className.ToString();
            }

            if (itemLevel is not 0)
            {
                newCharacter.ItemLevel = itemLevel;
            }

            if (!string.IsNullOrEmpty(engravings))
            {
                newCharacter.Engravings = Utils.ParseEngravings(engravings);
            }

            if (crit is not -1)
            {
                newCharacter.Crit = crit;
            }

            if (spec is not -1)
            {
                newCharacter.Spec = spec;
            }

            if (dom is not -1)
            {
                newCharacter.Dom = dom;
            }

            if (swift is not -1)
            {
                newCharacter.Swift = swift;
            }

            if (end is not -1)
            {
                newCharacter.End = end;
            }

            if (exp is not -1)
            {
                newCharacter.Exp = exp;
            }

            if (!string.IsNullOrEmpty(profilePicture))
            {
                newCharacter.ProfilePicture = profilePicture;
            }

            if (!string.IsNullOrEmpty(customMessage))
            {
                newCharacter.CustomProfileMessage = customMessage;
            }

            characterList.Add(newCharacter);
            characterList.Remove(oldCharacter);
            await JsonParsers.WriteCharactersAsync(characterList);

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = profilePicture == string.Empty ? Context.Guild.GetUser(newCharacter.DiscordUserId).GetAvatarUrl() : profilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", newCharacter.ItemLevel, true);
            embedBuilder.AddField("Class", newCharacter.ClassName, true);

            string engravingsString = "\u200b";
            foreach (string x in newCharacter.Engravings.Split(","))
            {
                engravingsString += x + "\n";
            }

            embedBuilder.AddField("Engravings", engravingsString, true);
            embedBuilder.AddField("Stats", $"Crit: {newCharacter.Crit}\nSpec: {newCharacter.Spec}\nDom: {newCharacter.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {newCharacter.Swift}\nEnd: {newCharacter.End}\nExp: {newCharacter.Exp}", true);

            if (newCharacter.CustomProfileMessage != string.Empty)
            {
                embedBuilder.AddField("Custom Message", newCharacter.CustomProfileMessage);
            }

            await RespondAsync(text: $"{characterName} got successfully updated", embed: embedBuilder.Build());
        }

        [SlashCommand("remove", "Removes the given character from your character list")]
        public async Task Remove([Summary("character-name", "Name of the character you want to remove")] string characterName)
        {
            ulong userId = Context.User.Id;
            List<Character> characterList = await JsonParsers.GetCharactersFromJsonAsync();
            Character character = characterList.Find(x => x.DiscordUserId == userId && x.CharacterName == characterName);

            if (character is null)
            {
                await RespondAsync(text: $"{characterName} is not registered or it doesn't belong to you", ephemeral: true);

                return;
            }

            characterList.Remove(character);

            await JsonParsers.WriteCharactersAsync(characterList);

            await RespondAsync(text: $"{characterName} has been successfully deleted", ephemeral: true);
        }

        [SlashCommand("profile", "Shows the profile of the given character")]
        public async Task Profile([Summary("character-name", "Name of the character")] string characterName)
        {
            EmbedBuilder embedBuilder = await CreateEmbedAsync(characterName, (character) =>
            {
                return Context.Guild.GetUser(character.DiscordUserId);
            });

            await RespondAsync(embed: embedBuilder.Build());
        }

        public async Task<EmbedBuilder> CreateEmbedAsync(string characterName, Func<Character, SocketGuildUser> getUser)
        {
            characterName = characterName.ToTitleCase();
            List<Character> characterList = await JsonParsers.GetCharactersFromJsonAsync();
            Character character = characterList.Find(x => x.CharacterName.ToLower() == characterName.ToLower());

            if (character is null)
            {
                await RespondAsync(text: $"{characterName} is not registered. You can register a character with **/register**", ephemeral: true);
                return null;
            }

            EmbedBuilder embedBuilder = new()
            {
                Title = $"Profile of {characterName}",
                ThumbnailUrl = character.ProfilePicture == string.Empty
                    ? getUser(character).GetAvatarUrl()
                    : character.ProfilePicture,
                Color = new Color(222, 73, 227),
            };

            embedBuilder.AddField("Item Level", character.ItemLevel, true);
            embedBuilder.AddField("Class", character.ClassName, true);

            string[] engravings = character.Engravings.Split(",");
            string engraving = "\u200b";

            foreach (string x in engravings)
            {
                engraving += x + "\n";
            }

            embedBuilder.AddField("Engravings", engraving, true);
            embedBuilder.AddField("Stats", $"Crit: {character.Crit}\nSpec: {character.Spec}\nDom: {character.Dom}", true);
            embedBuilder.AddField("\u200b", $"Swift: {character.Swift}\nEnd: {character.End}\nExp: {character.Exp}", true);

            if (character.CustomProfileMessage != string.Empty)
            {
                embedBuilder.AddField("Custom Message", character.CustomProfileMessage);
            }

            return embedBuilder;
        }

        [SlashCommand("engravings", "Edits the engravings of the given character")]
        public async Task Engravings([Summary("character-name", "Name of the character")] string characterName)
        {
            characterName = characterName.ToTitleCase();

            var chars = await JsonParsers.GetCharactersFromJsonAsync();

            var selectedChar = chars.Find(x => x.CharacterName.ToLower() == characterName.ToLower());

            if (selectedChar == null)
            {
                await RespondAsync($"No character named {characterName} was found", ephemeral: true);
                return;
            }

            var engString = Utils.ParseEngravings(selectedChar.Engravings);
            List<string> splitEngravings = engString.Split(",").ToList();

            var input1 = new TextInputBuilder().WithCustomId("eng1").WithLabel("Engraving 1").WithPlaceholder("e.g. Grudge 3").WithRequired(false).WithMaxLength(100);
            var input2 = new TextInputBuilder().WithCustomId("eng2").WithLabel("Engraving 2").WithPlaceholder("e.g. Grudge 3").WithRequired(false).WithMaxLength(100);
            var input3 = new TextInputBuilder().WithCustomId("eng3").WithLabel("Engraving 3").WithPlaceholder("e.g. Grudge 3").WithRequired(false).WithMaxLength(100);
            var input4 = new TextInputBuilder().WithCustomId("eng4").WithLabel("Engraving 4").WithPlaceholder("e.g. Grudge 3").WithRequired(false).WithMaxLength(100);
            var input5 = new TextInputBuilder().WithCustomId("eng5").WithLabel("Engraving 5").WithPlaceholder("e.g. Grudge 3").WithRequired(false).WithMaxLength(100);

            var inputBuilders = new List<TextInputBuilder>() { input1, input2, input3, input4, input5 };

            for (int i = 0; i < splitEngravings.Count; i++)
            {
                var eng = splitEngravings[i];
                var input = inputBuilders.Find(x => x.CustomId[3..] == (i + 1).ToString());
                input = input.WithValue(eng);
            }

            List<IMessageComponent> inputs = new();
            inputBuilders.ForEach(inp => inputs.Add(inp.Build()));

            var modal = new ModalBuilder()
                .WithCustomId($"eng:{characterName}")
                .WithTitle($"{characterName}'s Engravings")
                .AddTextInput(inputBuilders[0])
                .AddTextInput(inputBuilders[1])
                .AddTextInput(inputBuilders[2])
                .AddTextInput(inputBuilders[3])
                .AddTextInput(inputBuilders[4])
                .Build();

            await RespondWithModalAsync(modal);
        }
    }
}