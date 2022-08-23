using System.Collections.Generic;

namespace LostArkBot.databasemodels;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class User
{
    public User()
    {
        this.Characters = new HashSet<Character>();
        this.StaticGroups = new HashSet<StaticGroup>();
        this.Subscriptions = new HashSet<Subscription>();
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Id { get; set; }

    public ulong DiscordUserId { get; init; }

    public virtual ICollection<Character> Characters { get; }

    public virtual ICollection<StaticGroup> StaticGroups { get; }

    public virtual ICollection<Subscription> Subscriptions { get; }
}