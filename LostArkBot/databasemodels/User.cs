using System.Collections.Generic;

namespace LostArkBot.databasemodels
{
    public partial class User
    {
        public User()
        {
            Characters = new HashSet<Character>();
            StaticGroups = new HashSet<StaticGroup>();
            Subscriptions = new HashSet<Subscription>();
        }

        public int Id { get; set; }
        public ulong DiscordUserId { get; set; }

        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<StaticGroup> StaticGroups { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}