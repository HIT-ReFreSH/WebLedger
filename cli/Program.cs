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
using System;
using System.IO;
using System.Text.Json;

// 调试：检查配置读取
Console.WriteLine("=== 配置调试信息 ===");
Console.WriteLine("工作目录: " + Directory.GetCurrentDirectory());

string configPath = "config.json";
Console.WriteLine("配置文件路径: " + Path.GetFullPath(configPath));
Console.WriteLine("配置文件存在: " + File.Exists(configPath));

if (File.Exists(configPath))
{
    try
    {
        string configContent = File.ReadAllText(configPath);
        Console.WriteLine("配置文件内容:");
        Console.WriteLine(configContent);

        // 尝试解析 JSON
        var config = JsonSerializer.Deserialize<JsonElement>(configContent);
        if (config.TryGetProperty("host", out var host))
        {
            Console.WriteLine("解析到的 host: " + host.GetString());
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("配置文件解析错误: " + ex.Message);
    }
}
var builder = Suit.CreateBuilder();

builder.Configuration
       .AddEnvironmentVariables()
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
    var mysql = builder.Configuration["host-mysql"];
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