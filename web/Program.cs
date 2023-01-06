using HitReFreSH.WebLedger.Data;
using HitReFreSH.WebLedger.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var mysql = builder.Configuration.GetConnectionString("mysql");
// Add services to the container.
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IConfigManager, DirectConfigManager>();
builder.Services.AddScoped<ILedgerManager, DirectLedgerManager>();
builder.Services.AddLogging();
builder.Services.AddDbContext<LedgerContext>(
    c => c.UseMySql(
        mysql,
        new MySqlServerVersion(new Version(8, 0, 22)),
        b => b.MigrationsAssembly("WebLedger")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<LedgerContext>()
        .Database.MigrateAsync();
}


app.UseRouting();

app.MapControllers();

app.Run();