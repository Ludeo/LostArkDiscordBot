using PuppeteerSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostArkBot.Src.Bot.FileObjects.MetaGame
{
    public class ProfileScreenShot
    {
        public static async Task<bool> MakeProfileScreenshot(List<MetaEngraving> sortedEngravings, List<ArmorPiece> armorPieces, List<Accessory> accessories, MetaGameCharacter metaGameCharacter, MetaGameCharacterJson metaGameCharacterJson, string characterName)
        {
            string smokeImage = "https://lostark.meta-game.gg/smoke-bg.jpg";
            string classPreview = $"https://cdn.lostark.games.aws.dev/EFUI_IconAtlas/PC/{metaGameCharacter.ClassName.ToLower()}.png";
            string classIcon = $"https://lostark.meta-game.gg/ClassIcons/{metaGameCharacter.ClassName}.svg";

            string className = metaGameCharacter.ClassName;
            string characterLevel = metaGameCharacterJson.Level.ToString();
            string itemLevel = string.Format("{0:0.00}", metaGameCharacter.ItemLevel);
            string pvpRank = metaGameCharacterJson.PvpRank.ToString();
            string rosterLevel = metaGameCharacterJson.RosterLevel.ToString();
            string guildName = metaGameCharacterJson.GuildName;

            string crit = metaGameCharacterJson.Stats.First(x => x.Description == "Crit").Value;
            string spec = metaGameCharacterJson.Stats.First(x => x.Description == "Specialization").Value;
            string dom = metaGameCharacterJson.Stats.First(x => x.Description == "Domination").Value;
            string swift = metaGameCharacterJson.Stats.First(x => x.Description == "Swiftness").Value;
            string end = metaGameCharacterJson.Stats.First(x => x.Description == "Endurance").Value;
            string exp = metaGameCharacterJson.Stats.First(x => x.Description == "Expertise").Value;

            string engravingString = string.Empty;

            foreach (MetaEngraving engraving in sortedEngravings)
            {
                engravingString += $@"<div class=""engraving"" style=""display: flex; padding: 3px 3px;"" style=""display: flex;"">
                        <div class=""eng-img"" style=""height: 30px; width: 30px; margin-right: 10px;"">
                            <img src=""{engraving.Icon}"" style=""height: 30px; width: 30px;"">
                        </div>
                        <div class=""eng-text""
                            style=""display: flex; justify-content: space-between; width: 215px; align-items: center; color: #B4AEA9;"">
                            <div>{engraving.Name}</div>
                            <div style=""color: {(engraving.Penalty ? "red" : "#65aaec")};"">{(int)engraving.Value/5}</div>
                        </div>
                    </div>";
            }

            int height = 765;
            if (sortedEngravings.Count > 5)
            {
                height += (sortedEngravings.Count - 5) * 38;
            }

            #region htmlString
            string htmlString = $@"<html>

<body style=""margin: 0px"">
    <div class=""wrapper""
        style=""display: flex; width: 1000px; height: {height}px; background-color: #1E1E1D; box-sizing: border-box; font-family: sans-serif;"">
        <div class=""left-side"" style=""width: 40%; height: 100%;"">
            <div class=""char-image""
                style=""position: relative; width: 100%; height: 500px; background-image: url('{smokeImage}'); background-size: cover; background-repeat: no-repeat; background-position: center; display: flex; justify-content: center; align-items: center;"">
                <div class=""ilvl"" style=""position: absolute; top: 10px; right: 10px; color: wheat; font-size: 20px;"">
                    <div>Item Level</div>
                    <div>{itemLevel}</div>
                </div>
                <div class=""class-and-level""
                    style=""position: absolute; top: 10px; left: 10px; color: wheat; font-size: 20px;"">
                    <div>{className}</div>
                    <div>{characterLevel}</div>
                </div>
                <div class=""pvp-rank""
                    style=""position: absolute; bottom: 30px; left: 10px; color: wheat; font-size: 20px;"">
                    PVP Rank: {pvpRank}
                </div>
                <div class=""roster-lvl""
                    style=""position: absolute; bottom: 10px; left: 10px; color: wheat; font-size: 20px;"">
                    Roster Level: {rosterLevel}
                </div>
                <img src=""{classPreview}"" style=""height: 430px; width: 380px;"">
            </div>
            <div class=""class-icon""
                style=""position: relative; background-image: url('{classIcon}'); background-size: contain; background-repeat: no-repeat; background-position: center; height: calc(100% - 500px); display: flex; align-items: center; justify-content: center; flex-direction: column;"">
                <div class=""guild"" style=""font-size: 24px; color: wheat;"">
                    &lt{guildName}&gt
                </div>
                <div class=""char-name"" style=""font-size: 40px; color: #C0A669; font-family: auto;"">
                    {characterName}
                </div>
            </div>
        </div>
        <div class=""equipment-wrapper"" style=""display: flex; flex-direction: column; width: 60%;"">
            <div class=""items"" style=""display: flex; flex-direction: row; flex-wrap: wrap;"">
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(armorPieces.Count > 0 ? armorPieces[0].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(armorPieces.Count > 0 ? armorPieces[0].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(armorPieces.Count > 0 ? armorPieces[0].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(armorPieces.Count > 0 ? armorPieces[0].ItemLevel : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(accessories.Count > 0 ? accessories[0].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(accessories.Count > 0 ? accessories[0].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(accessories.Count > 0 ? accessories[0].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(accessories.Count > 0 ? accessories[0].Stat1 : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(accessories.Count > 0 ? accessories[0].Stat2 : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(armorPieces.Count > 1 ? armorPieces[1].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(armorPieces.Count > 1 ? armorPieces[1].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(armorPieces.Count > 1 ? armorPieces[1].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(armorPieces.Count > 1 ? armorPieces[1].ItemLevel : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(accessories.Count > 1 ? accessories[1].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(accessories.Count > 1 ? accessories[1].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(accessories.Count > 1 ? accessories[1].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(accessories.Count > 1 ? accessories[1].Stat1 : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(armorPieces.Count > 2 ? armorPieces[2].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(armorPieces.Count > 2 ? armorPieces[2].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(armorPieces.Count > 2 ? armorPieces[2].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(armorPieces.Count > 2 ? armorPieces[2].ItemLevel : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(accessories.Count > 2 ? accessories[2].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(accessories.Count > 2 ? accessories[2].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(accessories.Count > 2 ? accessories[2].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(accessories.Count > 2 ? accessories[2].Stat1 : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(armorPieces.Count > 3 ? armorPieces[3].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(armorPieces.Count > 3 ? armorPieces[3].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(armorPieces.Count > 3 ? armorPieces[3].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(armorPieces.Count > 3 ? armorPieces[3].ItemLevel : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(accessories.Count > 3 ? accessories[3].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(accessories.Count > 3 ? accessories[3].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(accessories.Count > 3 ? accessories[3].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(accessories.Count > 3 ? accessories[3].Stat1 : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(armorPieces.Count > 4 ? armorPieces[4].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(armorPieces.Count > 4 ? armorPieces[4].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(armorPieces.Count > 4 ? armorPieces[4].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(armorPieces.Count > 4 ? armorPieces[4].ItemLevel : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(accessories.Count > 4 ? accessories[4].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(accessories.Count > 4 ? accessories[4].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(accessories.Count > 4 ? accessories[4].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(accessories.Count > 4 ? accessories[4].Stat1 : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(armorPieces.Count > 5 ? armorPieces[5].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(armorPieces.Count > 5 ? armorPieces[5].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(armorPieces.Count > 5 ? armorPieces[5].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(armorPieces.Count > 5 ? armorPieces[5].ItemLevel : string.Empty)}</div>
                    </div>
                </div>
                <div class=""item""
                    style=""width: 49%; height: 80px; box-sizing: border-box; padding: 10px; display: flex; align-items: center; margin: 2px; background-color: #272726;"">
                    <div class=""image""
                        style=""height: 50px; width: 50px; border: 1px solid; margin-right: 10px; background-color: {(accessories.Count > 5 ? accessories[5].Color : "black")}; display: flex; align-items: center; justify-content: center;"">
                        <img src=""{(accessories.Count > 5 ? accessories[5].Icon : string.Empty)}"" style=""height: 50px; width: 50px;"">
                    </div>
                    <div class=""item-desc"" style=""display: flex; flex-direction: column; color: #B4AEA9"">
                        <div class=""item-name"" style=""font-weight: bold;"">{(accessories.Count > 5 ? accessories[5].Name : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(accessories.Count > 5 ? accessories[5].Engraving1.Replace("[", "").Replace("] ", "").Replace("Node", "") : string.Empty)}</div>
                        <div class=""item-ilvl"" style=""font-size: 13px;"">{(accessories.Count > 5 ? accessories[5].Engraving2.Replace("[", "").Replace("] ", "").Replace("Node", "") : string.Empty)}</div>
                    </div>
                </div>
            </div>
            <div class=""details-panel"" style=""width: 100%; display: flex;"">
                <div class=""engravings"" style=""width: 50%; padding: 10px;"">
                    <div class=""title""
                        style=""width: 100%;padding: 10px 0px;display: flex;justify-content: center;align-items: center;font-size: 16px;font-weight: bold;color: #C0A669;"">
                        Engravings from Gear</div>
                    {engravingString}
                </div>
                <div class=""combat-stats"" style=""width: 50%; padding: 10px;"">
                    <div class=""title""
                        style=""width: 100%;padding: 10px 0px;display: flex;justify-content: center;align-items: center;font-size: 16px;font-weight: bold;color: #C0A669;"">
                        Combat Stats</div>
                    <div class=""stat""
                        style=""display: flex; justify-content: space-between; padding: 8px 8px; color: {(int.Parse(crit) > 100 ? "#dcc896" : "#B4AEA9")};"">
                        <div>Crit</div>
                        <div>{crit}</div>
                    </div>
                    <div class=""stat""
                        style=""display: flex; justify-content: space-between; padding: 8px 8px; color: {(int.Parse(swift) > 100 ? "#dcc896" : "#B4AEA9")};"">
                        <div>Swiftness</div>
                        <div>{swift}</div>
                    </div>
                    <div class=""stat""
                        style=""display: flex; justify-content: space-between; padding: 8px 8px; color: {(int.Parse(spec) > 100 ? "#dcc896" : "#B4AEA9")};"">
                        <div>Specialization</div>
                        <div>{spec}</div>
                    </div>
                    <div class=""stat""
                        style=""display: flex; justify-content: space-between; padding: 8px 8px; color: {(int.Parse(dom) > 100 ? "#dcc896" : "#B4AEA9")};"">
                        <div>Domination</div>
                        <div>{dom}</div>
                    </div>
                    <div class=""stat""
                        style=""display: flex; justify-content: space-between; padding: 8px 8px; color: {(int.Parse(exp) > 100 ? "#dcc896" : "#B4AEA9")};"">
                        <div>Expertise</div>
                        <div>{exp}</div>
                    </div>
                    <div class=""stat""
                        style=""display: flex; justify-content: space-between; padding: 8px 8px; color: {(int.Parse(end) > 100 ? "#dcc896" : "#B4AEA9")};"">
                        <div>Endurance</div>
                        <div>{end}</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>

</html>
";
            #endregion

            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = true,
                    Args = new string[] { "--no-sandbox" },
#if DEBUG
#else
                    ExecutablePath = "/usr/bin/chromium-browser"
#endif
                });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlString);
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 1000,
                Height = height,
            });
            await page.ScreenshotAsync("image.png");

            return true;
        }
    }
}