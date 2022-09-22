using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArkBot.Bot.FileObjects;
using LostArkBot.Bot.Shared;
using LostArkBot.databasemodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LostArkBot.Bot.SlashCommands;

[Group("characters", "Commands to manage your characters")]
public class CharactersModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
{
    private readonly LostArkBotContext dbcontext;

    public CharactersModule(LostArkBotContext context) => this.dbcontext = context;

    [SlashCommand("add", "Adds a new character to your character list")]
    public async Task Add(
        [Summary("character-name", "Name of the character you want to register")] string characterName,
        [Summary("class-name", "Class of your character")]
        ClassName className,
        [Summary("item-level", "Item Level of your character")]
        int itemLevel,
        [Summary("engravings", "Engravings of your character, seperated by a comma")]
        string engravings = "",
        [Summary("crit", "How much Crit your character has")]
        int crit = 0,
        [Summary("spec", "How much Specialization your character has")]
        int spec = 0,
        [Summary("dom", "How much Domination your character has")]
        int dom = 0,
        [Summary("swift", "How much Swiftness your character has")]
        int swift = 0,
        [Summary("end", "How much Endurance your character has")]
        int end = 0,
        [Summary("exp", "How much Expertise your character has")]
        int exp = 0,
        [Summary("profile-picture", "Link for a profile picture")]
        string profilePicture = "",
        [Summary("custom-profile-message", "Custom message for your profile")]
        string customMessage = "")
    {
        await this.DeferAsync();

        characterName = characterName.ToTitleCase();

        Character oldCharacter =
            this.dbcontext.Characters.FirstOrDefault(x => EF.Functions.Collate(x.CharacterName, "latin1_general_ci") == characterName);

        if (oldCharacter is not null)
        {
            IMessage message = await this.FollowupAsync("auto-delete");
            await message.DeleteAsync();
            await this.FollowupAsync($"{characterName} exists already. You can update it with **/characters update**", ephemeral: true);

            return;
        }

        User user = this.dbcontext.Users.FirstOrDefault(x => x.DiscordUserId == this.Context.User.Id);

        if (user is null)
        {
            EntityEntry<User> userEntry = this.dbcontext.Users.Add(
                                                                   new User
                                                                   {
                                                                       DiscordUserId = this.Context.User.Id,
                                                                   });

            await this.dbcontext.SaveChangesAsync();
            user = userEntry.Entity;
        }

        Character newCharacter = new()
        {
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
            UserId = user.Id,
        };

        if (!string.IsNullOrEmpty(newCharacter.Engravings))
        {
            newCharacter.Engravings = Utils.ParseEngravings(engravings);
        }

        this.dbcontext.Characters.Add(newCharacter);
        await this.dbcontext.SaveChangesAsync();

        EmbedBuilder embedBuilder = new()
        {
            Title = $"Profile of {characterName}",
            ThumbnailUrl = profilePicture == string.Empty ? this.Context.Guild.GetUser(this.Context.User.Id).GetAvatarUrl() : profilePicture,
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

        await this.FollowupAsync($"{characterName} got successfully registered", embed: embedBuilder.Build());
    }

    [SlashCommand("engravings", "Edits the engravings of the given character")]
    public async Task Engravings([Summary("character-name", "Name of the character")] string characterName)
    {
        characterName = characterName.ToTitleCase();

        Character selectedChar =
            this.dbcontext.Characters.FirstOrDefault(x => EF.Functions.Collate(x.CharacterName, "latin1_general_ci") == characterName);

        if (selectedChar == null)
        {
            await this.RespondAsync($"No character named {characterName} was found", ephemeral: true);

            return;
        }

        string engString = Utils.ParseEngravings(selectedChar.Engravings);
        List<string> splitEngravings = engString.Split(",").ToList();

        TextInputBuilder input1 = new TextInputBuilder().WithCustomId("eng1").WithLabel("Engraving 1").WithPlaceholder("e.g. Grudge 3")
                                                        .WithRequired(false).WithMaxLength(100);

        TextInputBuilder input2 = new TextInputBuilder().WithCustomId("eng2").WithLabel("Engraving 2").WithPlaceholder("e.g. Grudge 3")
                                                        .WithRequired(false).WithMaxLength(100);

        TextInputBuilder input3 = new TextInputBuilder().WithCustomId("eng3").WithLabel("Engraving 3").WithPlaceholder("e.g. Grudge 3")
                                                        .WithRequired(false).WithMaxLength(100);

        TextInputBuilder input4 = new TextInputBuilder().WithCustomId("eng4").WithLabel("Engraving 4").WithPlaceholder("e.g. Grudge 3")
                                                        .WithRequired(false).WithMaxLength(100);

        TextInputBuilder input5 = new TextInputBuilder().WithCustomId("eng5").WithLabel("Engraving 5").WithPlaceholder("e.g. Grudge 3")
                                                        .WithRequired(false).WithMaxLength(100);

        List<TextInputBuilder> inputBuilders = new()
        {
            input1,
            input2,
            input3,
            input4,
            input5,
        };

        for (int i = 0; i < splitEngravings.Count; i++)
        {
            string eng = splitEngravings[i];
            TextInputBuilder input = inputBuilders.Find(x => x.CustomId[3..] == (i + 1).ToString());
            input.WithValue(eng);
        }

        Modal modal = new ModalBuilder()
                      .WithCustomId($"eng:{characterName}")
                      .WithTitle($"{characterName}'s Engravings")
                      .AddTextInput(inputBuilders[0])
                      .AddTextInput(inputBuilders[1])
                      .AddTextInput(inputBuilders[2])
                      .AddTextInput(inputBuilders[3])
                      .AddTextInput(inputBuilders[4])
                      .Build();

        await this.RespondWithModalAsync(modal);
    }

    [SlashCommand("list", "Shows all of your registered characters")]
    public async Task List()
    {
        await this.DeferAsync(true);

        ulong userId = this.Context.User.Id;
        List<Character> characters = this.dbcontext.Characters.Where(x => x.User.DiscordUserId == userId).ToList();

        if (characters.Count == 0)
        {
            await this.FollowupAsync("You don't have any characters added. You can add a character with **/characters add**", ephemeral: true);

            return;
        }

        EmbedBuilder embed = new()
        {
            Title = "Your characters",
            Color = Color.DarkPurple,
            Description = "\u200b",
            ThumbnailUrl = this.Context.User.GetAvatarUrl(),
        };

        List<GuildEmote> emotes = Program.GuildEmotes;

        foreach (Character character in characters)
        {
            GuildEmote emote = emotes.Find(x => x.Name == character.ClassName.ToLower());

            embed.AddField(
                           new EmbedFieldBuilder
                           {
                               Name = character.CharacterName,
                               Value = $"<:{emote.Name}:{emote.Id}> {character.ClassName}\n{character.ItemLevel}",
                               IsInline = true,
                           });
        }

        await this.FollowupAsync(embed: embed.Build(), ephemeral: true);
    }

    [SlashCommand("profile", "Shows the profile of the given character")]
    public async Task Profile([Summary("character-name", "Name of the character")] string characterName)
    {
        await this.DeferAsync();

        EmbedBuilder embedBuilder = Utils.CreateProfileEmbed(
                                                             characterName,
                                                             this.dbcontext,
                                                             character => this.Context.Guild.GetUser(character.User.DiscordUserId));

        if (embedBuilder is null)
        {
            IMessage deleteMessage = await this.FollowupAsync("auto-delete");
            await deleteMessage.DeleteAsync();
            await this.FollowupAsync($"{characterName} doesn't exist. You can add it with **/characters add**", ephemeral: true);

            return;
        }

        await this.FollowupAsync(embed: embedBuilder.Build());
    }

    [SlashCommand("remove", "Removes the given character from your character list")]
    public async Task Remove([Summary("character-name", "Name of the character you want to remove")] string characterName)
    {
        await this.DeferAsync(true);

        ulong userId = this.Context.User.Id;

        Character character = this.dbcontext.Characters
                                  .FirstOrDefault(x => x.User.DiscordUserId == userId && x.CharacterName == characterName);

        if (character is null)
        {
            await this.FollowupAsync($"{characterName} doesn't exist or it doesn't belong to you", ephemeral: true);

            return;
        }

        if (this.dbcontext.StaticGroups.FirstOrDefault(x => x.Characters.Contains(character)) is not null)
        {
            await this.FollowupAsync("You are still part of a static group, can't delete character unless you leave it", ephemeral: true);

            return;
        }

        this.dbcontext.Characters.Remove(character);
        await this.dbcontext.SaveChangesAsync();

        await this.FollowupAsync($"{characterName} has been successfully deleted", ephemeral: true);
    }

    [SlashCommand("update", "Updates the given character")]
    public async Task Update(
        [Summary("character-name", "Name of the character you want to register")] string characterName,
        [Summary("class-name", "Class of your character")]
        ClassName className = ClassName.Default,
        [Summary("item-level", "Item Level of your character")]
        int itemLevel = 0,
        [Summary("engravings", "Engravings of your character, seperated by a comma")]
        string engravings = null,
        [Summary("crit", "How much Crit your character has")]
        int crit = -1,
        [Summary("spec", "How much Specialization your character has")]
        int spec = -1,
        [Summary("dom", "How much Domination your character has")]
        int dom = -1,
        [Summary("swift", "How much Swiftness your character has")]
        int swift = -1,
        [Summary("end", "How much Endurance your character has")]
        int end = -1,
        [Summary("exp", "How much Expertise your character has")]
        int exp = -1,
        [Summary("profile-picture", "Link for a profile picture")]
        string profilePicture = "",
        [Summary("custom-profile-message", "Custom message for your profile")]
        string customMessage = "")
    {
        await this.DeferAsync();

        ulong discordUserId = this.Context.User.Id;
        characterName = characterName.ToTitleCase();

        Character character = this.dbcontext.Characters
                                  .Where(x => EF.Functions.Collate(x.CharacterName, "latin1_general_ci") == characterName)
                                  .Include(x => x.User)
                                  .FirstOrDefault();

        if (character is not null)
        {
            if (character.User.DiscordUserId != discordUserId)
            {
                IMessage message = await this.FollowupAsync("auto-delete");
                await message.DeleteAsync();
                await this.FollowupAsync($"You don't have permissions to update {characterName}", ephemeral: true);

                return;
            }
        }
        else
        {
            IMessage message = await this.FollowupAsync("auto-delete");
            await message.DeleteAsync();
            await this.FollowupAsync($"{characterName} doesn't exist. You can add it with **/characters add**", ephemeral: true);

            return;
        }

        if (className is not ClassName.Default)
        {
            character.ClassName = className.ToString();
        }

        if (itemLevel is not 0)
        {
            character.ItemLevel = itemLevel;
        }

        if (!string.IsNullOrEmpty(engravings))
        {
            character.Engravings = Utils.ParseEngravings(engravings);
        }

        if (crit is not -1)
        {
            character.Crit = crit;
        }

        if (spec is not -1)
        {
            character.Spec = spec;
        }

        if (dom is not -1)
        {
            character.Dom = dom;
        }

        if (swift is not -1)
        {
            character.Swift = swift;
        }

        if (end is not -1)
        {
            character.End = end;
        }

        if (exp is not -1)
        {
            character.Exp = exp;
        }

        if (!string.IsNullOrEmpty(profilePicture))
        {
            character.ProfilePicture = profilePicture;
        }

        if (!string.IsNullOrEmpty(customMessage))
        {
            character.CustomProfileMessage = customMessage;
        }

        this.dbcontext.Characters.Update(character);
        await this.dbcontext.SaveChangesAsync();

        EmbedBuilder embedBuilder = new()
        {
            Title = $"Profile of {characterName}",
            ThumbnailUrl = profilePicture == string.Empty ? this.Context.Guild.GetUser(character.User.DiscordUserId).GetAvatarUrl() : profilePicture,
            Color = new Color(222, 73, 227),
        };

        embedBuilder.AddField("Item Level", character.ItemLevel, true);
        embedBuilder.AddField("Class", character.ClassName, true);

        string engravingsString = "\u200b";

        foreach (string x in character.Engravings.Split(","))
        {
            engravingsString += x + "\n";
        }

        embedBuilder.AddField("Engravings", engravingsString, true);
        embedBuilder.AddField("Stats", $"Crit: {character.Crit}\nSpec: {character.Spec}\nDom: {character.Dom}", true);
        embedBuilder.AddField("\u200b", $"Swift: {character.Swift}\nEnd: {character.End}\nExp: {character.Exp}", true);

        if (character.CustomProfileMessage != string.Empty)
        {
            embedBuilder.AddField("Custom Message", character.CustomProfileMessage);
        }

        await this.FollowupAsync($"{characterName} got successfully updated", embed: embedBuilder.Build());
    }
}