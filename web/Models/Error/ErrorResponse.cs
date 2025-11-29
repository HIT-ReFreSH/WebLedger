namespace HitRefresh.WebLedger.Web.Models.Error;

/// <summary>
/// Wraps error details inside a root 'error' object
/// to provide a consistent API error response format.
/// </summary>
public sealed class ErrorResponse
{
    public ErrorDetail Error { get; init; } = default!;
}
