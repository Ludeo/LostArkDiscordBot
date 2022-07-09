using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using LostArkBot.Src.Bot.FileObjects;
using LostArkBot.Src.Bot.FileObjects.MetaGame;
using LostArkBot.Src.Bot.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.SlashCommands
{
    [DontAutoRegister]
    public class EngravingsModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private static readonly int SlotNumbers = 5;

        [SlashCommand("engravings2", "Edits the engravings of the given character")]
        public async Task Engravings([Summary("character-name", "Name of the character you want to update the engravings")] string characterName)
        {
            List<Character> characters = await JsonParsers.GetCharactersFromJsonAsync();
            Character character = characters.Find(x => x.CharacterName == characterName);

            List<Dictionary<string, Engraving>> engravings = await JsonParsers.GetEngravingsFromJsonAsync();
            List<Engraving> engravings2 = new();

            foreach(KeyValuePair<string, Engraving> valuePair in engravings.First())
            {
                engravings2.Add(valuePair.Value);
            }

            List<Engraving> engravings3 = engravings2.FindAll(x => x.Descriptions != null && x.Name != null);
            List<Engraving> neededEngravings = engravings3.FindAll(x => x.Descriptions.Length == 3 && x.Penalty == false);

            if(character == null)
            {
                await RespondAsync(text: "This character does not exist", ephemeral: true);
                return;
            }

            if(character.DiscordUserId != Context.User.Id)
            {
                await RespondAsync(text: "This character does not belong to you", ephemeral: true);
                return;
            }
            
            string content = "";

            SelectMenuBuilder engravingSlot = new SelectMenuBuilder()
                .WithCustomId("engravingslot");
            SelectMenuBuilder engravingMenu = new SelectMenuBuilder()
                .WithCustomId("engraving").WithDisabled(true);
            SelectMenuBuilder engravingLevel = new SelectMenuBuilder()
                .WithCustomId("engravinglevel").AddOption("Level 1", "1").AddOption("Level 2", "2").AddOption("Level 3", "3").WithDisabled(true);

            foreach(Engraving engraving in neededEngravings)
            {
                engravingMenu.AddOption(engraving.Name, engraving.Name);
            }

            for(int i = 1; i <= SlotNumbers; i++)
            {
                content += "Engraving " + i + ":\n";
                engravingSlot.AddOption("Slot " + i, i.ToString());
            }

            content += characterName;

            ButtonBuilder button = new ButtonBuilder().WithCustomId("submitbutton").WithLabel("Submit").WithStyle(ButtonStyle.Primary);
            ComponentBuilder components = new ComponentBuilder().WithSelectMenu(engravingSlot).WithSelectMenu(engravingMenu).WithSelectMenu(engravingLevel).WithButton(button);

            await RespondAsync(text: content, components: components.Build());
        }
    }

    public class EngravingsContextModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private static readonly int SlotNumbers = 5;

        [ComponentInteraction("submitbutton")]
        public async Task EngravingsSubmit()
        {
            string[] content = Context.Interaction.Message.Content.Split("\n");
            string characterName = content[SlotNumbers];

            List<Character> characterList = await JsonParsers.GetCharactersFromJsonAsync();
            Character character = characterList.Find(x => x.CharacterName == characterName);
            characterList.Remove(character);

            string regexContent = "";
            for(int i = 0; i < SlotNumbers; i++)
            {
                regexContent += content[i].Replace("*", "") + "\n";
            }
            Console.WriteLine(regexContent);
            Regex regex = new(@"^Engraving\s[1-6]:\s([\w\s]+)\s-\s([0-5])", RegexOptions.Multiline);
            MatchCollection regexResult = regex.Matches(regexContent);

            string engravings = "";
            foreach(Match match in regexResult)
            {
                string engravingName = match.Groups[1].Value;
                string engravingLevel = match.Groups[2].Value;
                engravings += engravingName + " " + engravingLevel + ",";
            }

            engravings = engravings[..^1];
            character.Engravings = engravings;

            characterList.Add(character);
            JsonParsers.WriteCharacters(characterList);

            await Context.Interaction.Message.DeleteAsync();
            await RespondAsync(text: "Successfully updated engravings for your character", ephemeral: true);
        }

        [ComponentInteraction("engravingslot")]
        public async Task EngravingsSlotMenu()
        {
            ComponentBuilder components = new();

            foreach(ActionRowComponent component in Context.Interaction.Message.Components)
            {
                IMessageComponent messageComponent = component.Components.First();

                if(messageComponent.Type == ComponentType.SelectMenu)
                {
                    SelectMenuComponent menuComponent = messageComponent as SelectMenuComponent;
                    SelectMenuBuilder menu = new(menuComponent);

                    if (menuComponent.CustomId == "engravingslot")
                    {
                        menu.WithDisabled(true).WithCustomId("engravingslot");
                    } else if(menuComponent.CustomId == "engraving")
                    {
                        menu.WithDisabled(false).WithCustomId("engraving");
                    } else if(menuComponent.CustomId == "engravinglevel")
                    {
                        menu.WithDisabled(true).WithCustomId("engravinglevel");
                    }

                    components.WithSelectMenu(menu);
                } else if(messageComponent.Type == ComponentType.Button)
                {
                    ButtonComponent buttonComponent = messageComponent as ButtonComponent;
                    ButtonBuilder button = new(buttonComponent);
                    button.WithDisabled(true);
                    components.WithButton(button);
                }
            }

            int engravingSlot = int.Parse(Context.Interaction.Data.Values.First());
            string[] content = Context.Interaction.Message.Content.Split("\n");
            string newContent = "";

            for(int i = 0; i < content.Length; i++)
            {
                if (content[i].Contains("*") && i == engravingSlot - 1)
                {
                    newContent += content[i] + "\n";
                } else if (content[i].Contains("*"))
                {
                    newContent += content[i][2..^4] + "\n";
                } else if(i == engravingSlot - 1)
                {
                    newContent += "**" + content[i] + "**" + "\n";
                } else
                {
                    newContent += content[i] + "\n";
                }
            }

            await Context.Interaction.Message.ModifyAsync(x => {
                x.Content = newContent;
                x.Components = components.Build();
            });

            try
            {
                await RespondAsync();
            }
            catch (HttpException exception)
            {
                await LogService.Log(LogSeverity.Error, this.GetType().Name, exception.Message);
            }
        }

        [ComponentInteraction("engraving")]
        public async Task Engraving()
        {
            ComponentBuilder components = new();

            foreach (ActionRowComponent component in Context.Interaction.Message.Components)
            {
                IMessageComponent messageComponent = component.Components.First();

                if (messageComponent.Type == ComponentType.SelectMenu)
                {
                    SelectMenuComponent menuComponent = messageComponent as SelectMenuComponent;
                    SelectMenuBuilder menu = new(menuComponent);

                    if (menuComponent.CustomId == "engravingslot")
                    {
                        menu.WithDisabled(true).WithCustomId("engravingslot");
                    }
                    else if (menuComponent.CustomId == "engraving")
                    {
                        menu.WithDisabled(true).WithCustomId("engraving");
                    }
                    else if (menuComponent.CustomId == "engravinglevel")
                    {
                        menu.WithDisabled(false).WithCustomId("engravinglevel");
                    }

                    components.WithSelectMenu(menu);
                }
                else if (messageComponent.Type == ComponentType.Button)
                {
                    ButtonComponent buttonComponent = messageComponent as ButtonComponent;
                    ButtonBuilder button = new(buttonComponent);
                    button.WithDisabled(true);
                    components.WithButton(button);
                }
            }

            string[] content = Context.Interaction.Message.Content.Split("\n");
            string newContent = "";

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Contains("*"))
                {
                    newContent += "**Engraving " + (i + 1) + ": " + Context.Interaction.Data.Values.First() + "**\n";
                }
                else
                {
                    newContent += content[i] + "\n";
                }
            }

            await Context.Interaction.Message.ModifyAsync(x => {
                x.Content = newContent;
                x.Components = components.Build();
            });

            try
            {
                await RespondAsync();
            }
            catch (HttpException exception)
            {
                await LogService.Log(LogSeverity.Error, this.GetType().Name, exception.Message);
            }
        }

        [ComponentInteraction("engravinglevel")]
        public async Task EngravingLevel()
        {
            ComponentBuilder components = new();

            foreach (ActionRowComponent component in Context.Interaction.Message.Components)
            {
                IMessageComponent messageComponent = component.Components.First();

                if (messageComponent.Type == ComponentType.SelectMenu)
                {
                    SelectMenuComponent menuComponent = messageComponent as SelectMenuComponent;
                    SelectMenuBuilder menu = new(menuComponent);

                    if (menuComponent.CustomId == "engravingslot")
                    {
                        menu.WithDisabled(false).WithCustomId("engravingslot");
                    }
                    else if (menuComponent.CustomId == "engraving")
                    {
                        menu.WithDisabled(true).WithCustomId("engraving");
                    }
                    else if (menuComponent.CustomId == "engravinglevel")
                    {
                        menu.WithDisabled(true).WithCustomId("engravinglevel");
                    }

                    components.WithSelectMenu(menu);
                }
                else if (messageComponent.Type == ComponentType.Button)
                {
                    ButtonComponent buttonComponent = messageComponent as ButtonComponent;
                    ButtonBuilder button = new(buttonComponent);
                    button.WithDisabled(false);
                    components.WithButton(button);
                }
            }

            string[] content = Context.Interaction.Message.Content.Split("\n");
            string newContent = "";

            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Contains("*"))
                {
                    newContent += content[i][..^2] + " - " + Context.Interaction.Data.Values.First() + "**\n";
                }
                else
                {
                    newContent += content[i] + "\n";
                }
            }

            await Context.Interaction.Message.ModifyAsync(x => {
                x.Content = newContent;
                x.Components = components.Build();
            });

            try
            {
                await RespondAsync();
            }
            catch (HttpException exception)
            {
                await LogService.Log(LogSeverity.Error, this.GetType().Name, exception.Message);
            }
        }
    }
}