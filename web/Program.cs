using HitRefresh.WebLedger.Data;
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

// Add CORS support for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddScoped<IConfigManager, DirectConfigManager>();
builder.Services.AddScoped<ILedgerManager, DirectLedgerManager>();
builder.Services.AddLogging();

builder.Services.AddSingleton<AccessMiddleware>();
builder.Services.AddDbContext<LedgerContext>(
    c => c.UseMySql(
        mysql,
        ServerVersion.AutoDetect(mysql),
        b => b.MigrationsAssembly("HitRefresh.WebLedger")));
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

// Enable CORS
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.UseMiddleware<AccessMiddleware>();
app.MapRazorPages();

app.MapControllers();

app.Run();