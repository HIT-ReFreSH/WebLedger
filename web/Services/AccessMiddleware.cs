using HitReFreSH.WebLedger.Data;

namespace HitReFreSH.WebLedger.Web.Services;

public class AccessMiddleware : IMiddleware
{
    private Dictionary<string, string> _access=new();
    private readonly IServiceProvider _serviceProvider;

    public AccessMiddleware( IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_access.Any())
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
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
                await using var scope = _serviceProvider.CreateAsyncScope();
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