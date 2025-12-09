using HitRefresh.WebLedger.Data;

namespace HitRefresh.WebLedger.Web.Services;

public class AccessMiddleware(IServiceProvider serviceProvider) : IMiddleware
{
    private Dictionary<string, string> _access=new();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Allow health checks without authentication
        if (context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }
        if (!_access.Any())
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            _access = scope.ServiceProvider.GetRequiredService<LedgerContext>()
                .Access.ToDictionary(x => x.Name, x => x.Key);
            if (!_access.Any() || (
                    context.Request.Headers.TryGetValue("wl-access", out var accessName2) &&
                    context.Request.Headers.TryGetValue("wl-secret", out var secret2) &&
                    _access.Any(a => a.Key == accessName2.First() && a.Value == secret2.First())
                )
               )
                await next(context);
            else
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
        }
        else
        {
            if (context.Request.Headers.TryGetValue("wl-access", out var accessName) &&
                context.Request.Headers.TryGetValue("wl-secret", out var secret) &&
                _access.Any(a => a.Key == accessName.First() && a.Value == secret.First())
                )
                await next(context);
            else
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                _access = scope.ServiceProvider.GetRequiredService<LedgerContext>()
                    .Access.ToDictionary(x => x.Name, x => x.Key);
                if (!_access.Any() || (
                        context.Request.Headers.TryGetValue("wl-access", out var accessName2) &&
                        context.Request.Headers.TryGetValue("wl-secret", out var secret2) &&
                        _access.Any(a => a.Key == accessName2.First() && a.Value == secret2.First())
                    )
                   )
                    await next(context);
                else
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
        }


    }
}