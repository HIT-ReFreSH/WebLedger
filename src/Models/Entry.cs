namespace HitReFreSH.WebLedger.Models;

public record Entry(decimal Amount, DateTime GivenTime, string Type, string? Category, string? Description)
{
}
public record RecordedEntry(Guid Id, decimal Amount, DateTime GivenTime, string Type, string? Category, string? Description) : Entry(Amount, GivenTime, Type, Category, Description){ }