namespace HitRefresh.WebLedger.Web.Models.Error;

/// <summary>
/// Represents a single structured error description returned to API clients.
/// </summary>
public sealed class ErrorDetail
{
    public string Code { get; init; } = default!;
    public string Message { get; init; } = default!;
    public string? Details { get; init; }
    public string RequestId { get; init; } = default!;
}
