using HitReFreSH.WebLedger.Data;
using HitReFreSH.WebLedger.Services;
using HitReFreSH.WebLedger.Web.Services;
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
builder.Services.AddSingleton<AccessMiddleware>();
builder.Services.AddDbContext<LedgerContext>(
    c => c.UseMySql(
        mysql,
        new MySqlServerVersion(new Version(8, 0, 22)),
        b => b.MigrationsAssembly("WebLedger")));
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();

}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

Dictionary<string, string> access;
await using (var scope = app.Services.CreateAsyncScope())
{
    var context= scope.ServiceProvider.GetRequiredService<LedgerContext>();
        await context .Database.MigrateAsync();
        access=context.Access.ToDictionary(x => x.Name, x => x.Key);
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseMiddleware<AccessMiddleware>();
app.MapRazorPages();

app.MapControllers();

app.Run();