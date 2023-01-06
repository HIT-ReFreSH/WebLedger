using HitReFreSH.WebLedger.Data;

namespace HitReFreSH.WebLedger.Web.Services;

public class AccessMiddleware : IMiddleware
{
    private readonly LedgerContext _database;

    public AccessMiddleware(LedgerContext database)
    {
        _database = database;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_database.Access.Any() || (
                context.Request.Headers.TryGetValue("wl-access", out var accessName) &&
                context.Request.Headers.TryGetValue("wl-secret", out var secret) &&
                _database.Access.Any(a => a.Name == accessName && a.Key == secret)
            )
           )
            await next(context);

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
    }
}