using HitRefresh.WebLedger.Data;
using HitRefresh.WebLedger.Web.Services.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Linq;
using HitRefresh.WebLedger.Services;
using HitRefresh.WebLedger.Web.Services;
using Microsoft.EntityFrameworkCore;
using Steeltoe.Extensions.Configuration.Placeholder;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddPlaceholderResolver();

var mysql = builder.Configuration["ConnectionStrings:mysql"];
//Console.WriteLine(mysql);
// Add services to the container.
builder.Services.AddScoped<AccessMiddleware>();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IConfigManager, DirectConfigManager>();
builder.Services.AddScoped<ILedgerManager, DirectLedgerManager>();
builder.Services.AddLogging();

builder.Services.AddSingleton<AccessMiddleware>();
// Health checks
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks()
    .AddCheck<LedgerDbHealthCheck>("database")
    .AddCheck<ServiceAvailabilityHealthCheck>("external_service");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 22));
builder.Services.AddDbContext<LedgerContext>(
    c => c.UseMySql(
        mysql,
        serverVersion,
        b => b.MigrationsAssembly("WebLedger")));
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMiddleware<ErrorHandlingMiddleware>();

}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<LedgerContext>();
    await context.Database.MigrateAsync();
}



app.UseStaticFiles();
app.UseRouting();

// Health check endpoints - expose before AccessMiddleware so external monitors can access
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/detailed", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                exception = e.Value.Exception?.Message,
                details = e.Value.Data.Count > 0 ? e.Value.Data.ToDictionary(k => k.Key, v => v.Value) : null,
                duration = e.Value.Duration.ToString()
            })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }
});

app.UseAuthorization();
app.UseMiddleware<AccessMiddleware>();
app.MapRazorPages();

app.MapControllers();

app.Run();