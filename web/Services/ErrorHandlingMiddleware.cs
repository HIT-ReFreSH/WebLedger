using HitRefresh.WebLedger.Web.Models.Error;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HitRefresh.WebLedger.Web.Services;

using System.Diagnostics;
using HitRefresh.WebLedger.Web.Models.Error;
using Microsoft.AspNetCore.Http;


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
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, errorCode, message) = ExceptionErrorMapper.Map(ex);

        var error = new ErrorResponse
        {
            Error = new ErrorDetail
            {
                Code = errorCode,
                Message = message,
                Details = ex.Message,
                RequestId = requestId
            }
        };

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(error);
    }


}



