using System.Collections.Generic;

namespace LostArkBot.databasemodels;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Character
{
    public Character() => this.StaticGroups = new HashSet<StaticGroup>();

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Id { get; set; }

    public string CharacterName { get; init; }

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

    public int UserId { get; init; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public virtual User User { get; set; }

    public virtual ICollection<StaticGroup> StaticGroups { get; }
}