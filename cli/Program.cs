using HitRefresh.MobileSuit;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HitRefresh.WebLedger.CLI.Services;
using HitRefresh.WebLedger.Services;
using System.Net.Http;
using HitRefresh.WebLedger.Data;
using Microsoft.EntityFrameworkCore;
using HitRefresh.WebLedger.CLI;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.Placeholder;

var builder = Suit.CreateBuilder();

builder.Configuration
       .AddJsonFile("config.json")
       .AddPlaceholderResolver();
builder.Services.AddLogging();
builder.Services.AddScoped<WebGuiHelper>();
if (builder.Configuration["target"] == "http")
{
    var http = new HttpClient();
    http.BaseAddress = new(builder.Configuration["host"]);
    http.DefaultRequestHeaders.Add("wl-access",
        builder.Configuration["access"]);
    http.DefaultRequestHeaders.Add("wl-secret",
        builder.Configuration["secret"]);

    builder.Services.AddSingleton(http);
    builder.Services.AddScoped<ILedgerManager, HttpLedgerManager>();
    builder.Services.AddScoped<IConfigManager, HttpConfigManager>();
}
else
{
    var mysql = builder.Configuration["host-sql"];
    builder.Services.AddScoped<IConfigManager, DirectConfigManager>();
    builder.Services.AddScoped<ILedgerManager, DirectLedgerManager>();

    builder.Services.AddDbContext<LedgerContext>(
        c => c.UseMySql(
            mysql,
            new MySqlServerVersion(new Version(8, 0, 22)),
            b => b.MigrationsAssembly("LibWebLedger")));
}

builder.MapClient<Driver>();
builder.Use4BitColorIO();
builder.UsePowerLine();
var app = builder.Build();
await app.RunAsync();