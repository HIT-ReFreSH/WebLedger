namespace HitReFreSH.WebLedger.Models;

public record Entry(decimal Amount, DateTime GivenTime, string Type, string? Category, string? Description)
{
}