namespace HitRefresh.WebLedger.Web.Models.Error;

public sealed class ErrorResponse
{
    public ErrorDetail Error { get; init; } = default!;
}
