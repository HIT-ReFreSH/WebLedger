using HitReFreSH.WebLedger.Models;
using HitReFreSH.WebLedger.Services;
using PlasticMetal.MobileSuit;
using PlasticMetal.MobileSuit.UI;

namespace HitReFreSH.WebLedger.CLI;

[SuitInfo("WebLedger")]
public class Driver
{
    private IIOHub IO { get; }
    private readonly IConfigManager _config;
    private readonly ILedgerManager _ledger;
    public Driver(IConfigManager config, ILedgerManager ledger, IIOHub iO)
    {
        IO = iO;
        _config = config;
        _ledger = ledger;
    }
    [SuitAlias("add")]
    public async Task Insert()
    {
        var amount = decimal.Parse(await IO.ReadLineAsync("Input Amount") ?? "0");
        var type = await IO.ReadLineAsync("Input Type") ?? "";
        var timeExpr = await IO.ReadLineAsync("Input Time (yyMMdd)");

        var time =  string.IsNullOrEmpty(timeExpr)?DateTime.Now:
            new DateTime(int.Parse(timeExpr[..2]) + 2000, int.Parse(timeExpr[2..4]), int.Parse(timeExpr[4..]));
        var categories = await _ledger.GetAllCategories();
        var category = IO.CuiSelectItemFrom("Input Category", new[] { "(auto)" }.Concat(categories.Select(c => c.Name)).ToArray());
        category = category.Replace("(auto)", "");
        var desc = await IO.ReadLineAsync("Input Description") ?? "";
        var entry = new Entry(amount, time, type, category, desc);
        var id = await _ledger.Insert(entry);
        await IO.WriteLineAsync($"Added: {id}");
    }
    [SuitAlias("cat")]
    public async Task AddCategory(string catname, string? supcat = null)
    {
        await _ledger.AddOrUpdateCategory(new(catname, supcat));
    }
    [SuitAlias("del-cat")]
    public async Task DeleteCategory(string catname)
    {
        await _ledger.RemoveCategory(catname);
    }
    [SuitAlias("grant")]
    public async Task<string> GrantAccess(string access)
    {
        return await _config.AddAccess(access);
    }
    [SuitAlias("ungrant")]
    public async Task CancelAccess(string access)
    {
        await _config.RemoveAccess(access);
    }
    [SuitAlias("ls-acc")]
    public async Task GetAccess()
    {
        var access = await _config.GetAllAccess();
        foreach (var a in access)
        {
            await IO.WriteLineAsync($"{a.Name}\t{a.Key}");
        }
    }
    [SuitAlias("ls-all")]
    public async Task GetAll()
    {
        var entries = await _ledger.Select(new(DateTime.MinValue, DateTime.Now, null, null));
        await IO.WriteLineAsync("Type\tAmount\tCategory\tTime\tDescription");
        foreach (var e in entries.OrderBy(e=>e.GivenTime))
        {
            await IO.WriteLineAsync($"{e.Type}\t{e.Amount}\t{e.Category}\t{e.GivenTime:yy-MM-dd}\t{e.Description}");
        }
    }
}