using Xunit;
using HitRefresh.WebLedger;
using HitRefresh.WebLedger.Web.Models.Error;
using HitRefresh.WebLedger.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text;
using System.Text.Json;


namespace WebLedger.Tests;

public class ErrorHandlingMiddlewareTests
{
    private static async Task<ErrorResponse> InvokeAndGetErrorResponseAsync(
        Func<Task> throwingDelegate,
        int expectedStatusCode)
    {
        // Arrange HttpContext + Response-Stream
        var context = new DefaultHttpContext();
        var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        // Fake-RequestDelegate, which throws the desired exception
        RequestDelegate next = _ =>
        {
            return throwingDelegate();
        };

        var logger = NullLogger<ErrorHandlingMiddleware>.Instance;
        var middleware = new ErrorHandlingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert StatusCode
        Assert.Equal(expectedStatusCode, context.Response.StatusCode);

        // Read Response-Body 
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        var error = JsonSerializer.Deserialize<ErrorResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(error);
        Assert.NotNull(error!.Error);

        return error!;
    }

    [Fact]
    public async Task TypeUndefinedException_Returns400WithTypeUndefinedCode()
    {
        // Arrange & Act
        var error = await InvokeAndGetErrorResponseAsync(
            throwingDelegate: () => Task.FromException(new TypeUndefinedException("demo-type")),
            expectedStatusCode: StatusCodes.Status400BadRequest);

        // Assert
        Assert.Equal("TYPE_UNDEFINED", error.Error!.Code);
        Assert.Equal("The specified type does not exist.", error.Error.Message);
        Assert.False(string.IsNullOrWhiteSpace(error.Error.RequestId));
    }

    [Fact]
    public async Task ViewTemplateUndefinedException_Returns404WithViewTemplateUndefinedCode()
    {
        var error = await InvokeAndGetErrorResponseAsync(
            throwingDelegate: () => Task.FromException(new ViewTemplateUndefinedException("demo-view")),
            expectedStatusCode: StatusCodes.Status404NotFound);

        Assert.Equal("VIEW_TEMPLATE_UNDEFINED", error.Error!.Code);
        Assert.Equal("The specified view template does not exist.", error.Error.Message);
    }

    [Fact]
    public async Task GenericException_Returns500WithInternalServerErrorCode()
    {
        var error = await InvokeAndGetErrorResponseAsync(
            throwingDelegate: () => Task.FromException(new Exception("boom")),
            expectedStatusCode: StatusCodes.Status500InternalServerError);

        Assert.Equal("INTERNAL_SERVER_ERROR", error.Error!.Code);
        Assert.Equal("An unexpected error occurred.", error.Error.Message);
    }
}
