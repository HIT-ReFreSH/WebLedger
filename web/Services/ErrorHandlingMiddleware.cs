using HitRefresh.WebLedger.Web.Models.Error;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HitRefresh.WebLedger.Web.Services;

using System.Diagnostics;
using HitRefresh.WebLedger.Web.Models.Error;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing request {Path}", context.Request.Path);

            await WriteErrorResponseAsync(context, ex);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        // RequestId: use Activity.Id or TraceIdentifier
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;

        var error = new ErrorResponse
        {
            Error = new ErrorDetail
            {
                Code = "INTERNAL_SERVER_ERROR",
                Message = "An unexpected error occurred.",
                Details = ex.Message,  // only for Development
                RequestId = requestId
            }
        };

        // If something has already been written, reset Response  (!!)
        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(error);

        // TODO : refining the mapping(TYPE_UNDEFINED, 404, 400 etc.).
    }
}
