using System.Collections.Generic;

namespace LostArkBot.databasemodels;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StaticGroup
{
    public StaticGroup() => this.Characters = new HashSet<Character>();

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Id { get; set; }

    public string Name { get; init; }

    public int LeaderId { get; init; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public virtual User Leader { get; set; }

    public virtual ICollection<Character> Characters { get; }
}