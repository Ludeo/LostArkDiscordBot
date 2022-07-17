using System;
using System.Collections.Generic;

namespace LostArkBot.databasemodels
{
    public partial class Character
    {
        public int Id { get; set; }
        public string CharacterName { get; set; }
        public long DiscordUserId { get; set; }
        public string ClassName { get; set; }
        public int ItemLevel { get; set; }
        public string Engravings { get; set; }
        public int? Crit { get; set; }
        public int? Spec { get; set; }
        public int? Dom { get; set; }
        public int? Swift { get; set; }
        public int? End { get; set; }
        public int? Exp { get; set; }
        public string ProfilePicture { get; set; }
        public string CustomProfileMessage { get; set; }
    }
}
