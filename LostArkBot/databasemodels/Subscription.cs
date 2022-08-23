namespace LostArkBot.databasemodels;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Subscription
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int UserId { get; set; }

    public int ItemId { get; init; }

    public virtual User User { get; init; }
}