namespace HitRefresh.WebLedger.Models;

public record SelectOption(DateTime StartTime, DateTime EndTime, bool? Direction, string? Category)
{
}