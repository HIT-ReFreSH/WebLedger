using System.Drawing;
using HitReFreSH.WebLedger.Data;
using HitReFreSH.WebLedger.Models;
using HitReFreSH.WebLedger.Services;
using PlasticMetal.MobileSuit;
using PlasticMetal.MobileSuit.UI;
using System.Xml.Linq;
using HitReFreSH.WebLedger.CLI.Services;

namespace HitReFreSH.WebLedger.CLI;

[SuitInfo("WebLedger")]
public class Driver
{
    private IIOHub IO { get; }
    private readonly IConfigManager _config;
    private readonly ILedgerManager _ledger;
    private readonly WebGuiHelper _webGui;

    public Driver(IConfigManager config, ILedgerManager ledger, IIOHub iO,WebGuiHelper webGui)
    {
        IO = iO;
        _config = config;
        _ledger = ledger;
        _webGui = webGui;
    }
    [SuitAlias("add")]
    public async Task Insert()
    {
        var amount = decimal.Parse(await IO.ReadLineAsync("Input Amount") ?? "0");
        var type = await IO.ReadLineAsync("Input Type") ?? "";
        var timeExpr = await IO.ReadLineAsync("Input Time (yyMMdd)");

        var time = string.IsNullOrEmpty(timeExpr) ? DateTime.Now :
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
        foreach (var e in entries.OrderBy(e => e.GivenTime))
        {
            await IO.WriteLineAsync($"{e.Type}\t{e.Amount}\t{e.Category}\t{e.GivenTime:yy-MM-dd}\t{e.Description}");
        }
    }

    [SuitAlias("auto")]
    public async Task EnableViewAutomation(string type)
    {
        var tmpls = await _ledger.GetAllViewTemplateNames();
        var tmpl = IO.CuiSelectItemFrom("Select Template", tmpls.ToArray());
        await _ledger.EnableViewAutomation(new(type.ToLower()[0] switch
        {
            'd' => LedgerViewAutomationType.Daily,
            'w' => LedgerViewAutomationType.Weekly,
            'm' => LedgerViewAutomationType.Monthly,
            'q' => LedgerViewAutomationType.Quarterly,
            'y' => LedgerViewAutomationType.Yearly,
            _ => throw new ArgumentOutOfRangeException()
        }, tmpl));
    }
    [SuitAlias("rm-auto")]
    public async Task DisableViewAutomation()
    {
        var atms = await _ledger.GetAllViewAutomation();

        var atm = IO.CuiSelectItemFrom("Select Automation", (new[] { new ViewAutomation(0, "(cancel)") }.Concat(atms)).ToArray());
        if (atm.TemplateName != "(cancel)")
            await _ledger.DisableViewAutomation(atm);
    }
    [SuitAlias("tmpl")]
    public async Task AddViewTemplate()
    {
        var name = await IO.ReadLineAsync("Input template name");
        if (name is null) return;
        var isIncome =  IO.CuiYesNo("Is Income?", false);
        var cats = await _ledger.GetAllCategories();
        var selected = IO.CuiSelectItemsFrom("Select categories", cats.Select(c=>c.Name).ToArray()).ToArray();
        await _ledger.AddOrUpdateViewTemplate(new(name, selected, isIncome));

    }
    [SuitAlias("ed-tmpl")]
    public async Task UpdateViewTemplate()
    {
        var tmpls = await _ledger.GetAllViewTemplateNames();
        var tmpl = IO.CuiSelectItemFrom("Select Template", tmpls.ToArray());
        var origin = await _ledger.GetViewTemplate(tmpl);
        var isIncome = IO.CuiYesNo("Is Income?", origin.IsIncome);
        var cats = (await _ledger.GetAllCategories());
        var originSelected = origin.Categories.ToHashSet();
        var selected = IO.CuiSelectItemsFrom("Select categories", cats.Select(c => c.Name).ToArray(),
            c => originSelected.Contains(c) ? $"+{c}" : $"-{c}").ToArray();
        await _ledger.AddOrUpdateViewTemplate(new(tmpl, selected, isIncome));
    }
    [SuitAlias("rm-tmpl")]
    public async Task RemoveViewTemplate(string template)
    {
        if (string.IsNullOrEmpty(template)) return;
        await _ledger.RemoveViewTemplate(template);
    }
    [SuitAlias("ad-view")]
    public async Task AddView(string name)
    {
        var tmpls = await _ledger.GetAllViewTemplateNames();
        var tmpl = IO.CuiSelectItemFrom("Select Template", tmpls.ToArray());

        var timeExpr = await IO.ReadLineAsync("Input Start Time (yyMMdd, today default)");

        var startTime = string.IsNullOrEmpty(timeExpr) ? DateTime.Today :
            new DateTime(int.Parse(timeExpr[..2]) + 2000, int.Parse(timeExpr[2..4]), int.Parse(timeExpr[4..]));
        timeExpr = await IO.ReadLineAsync("Input End Time (yyMMdd,tomorrow default)");

        var endTime = string.IsNullOrEmpty(timeExpr) ? DateTime.Today.AddDays(1) :
            new DateTime(int.Parse(timeExpr[..2]) + 2000, int.Parse(timeExpr[2..4]), int.Parse(timeExpr[4..]));
        await _ledger.AddView(new(tmpl,startTime,endTime,tmpl));
    }
    [SuitAlias("rm-view")]
    public async Task RemoveView()
    {
        var views = await _ledger.GetAllViewNames();
        var view = IO.CuiSelectItemFrom("Select View", views.ToArray());
        await _ledger.RemoveView(view);
    }
    [SuitAlias("ls-cat")]
    public async Task GetAllCategories()
    {
        var views = await _ledger.GetAllCategories();
        foreach (var view in views)
        {
            await IO.WriteLineAsync(view.Name);
        }
    }
    [SuitAlias("ls-view")]
    public async Task GetAllViewNames()
    {
        var views = await _ledger.GetAllViewNames();
        foreach (var view in views)
        {
            await IO.WriteLineAsync(view);
        }
    }

    [SuitAlias("ls-tmpl")]
    public async Task GetAllViewTemplateNames()
    {
        var views = await _ledger.GetAllViewTemplateNames();
        foreach (var view in views)
        {
            await IO.WriteLineAsync(view);
        }
    }

    [SuitAlias("dt-tmpl")]
    public async Task GetViewTemplate(string name)
    {
        var tmpl=await _ledger.GetViewTemplate(name);
        await IO.WriteLineAsync($"Direction: {(tmpl.IsIncome?"Income":"Outcome")}");
        await IO.WriteLineAsync($"Categories: {(string.Join(", ",tmpl.Categories))}");
    }

    [SuitAlias("ls-auto")]
    public async Task GetAllViewAutomation()
    {
        var views = await _ledger.GetAllViewAutomation();
        await IO.WriteLineAsync("Type\tTemplate");
        foreach (var view in views)
        {
            await IO.WriteLineAsync($"{Enum.GetName( view.Type)}\t{view.TemplateName}");
        }
    }

    [SuitAlias("gq")]
    public async Task QueryGraphical()
    {
        var views = await _ledger.GetAllViewNames();
        var view = IO.CuiSelectItemFrom("Select View", views.ToArray());
        await _webGui.CachedQueryGraphical(new(view, 10));
    }
    [SuitAlias("q")]
    public async Task Query()
    {
        var views = await _ledger.GetAllViewNames();
        var view = IO.CuiSelectItemFrom("Select View", views.ToArray());
        var (raw,catD,timeD)= await _ledger.Query(new(view, 10));
        var cat = catD
            .Select(c => (c.Key, c.Value))
            .OrderByDescending(c => c.Value)
            .ToArray();
        var time = timeD
            .Select(c => (c.Key, c.Value))
            .OrderByDescending(c => c.Key)
            .ToArray();
        if (raw.Count == 0)
        {
            await IO.WriteLineAsync("No Data Found", OutputType.Warning);
            return;
        }
        await IO.WriteLineAsync("Raw:", OutputType.Title);
        IO.AppendWriteLinePrefix();
        await IO.WriteLineAsync("Type\tAmount\tCategory\tTime\tDescription");
        foreach (var e in raw.OrderBy(e => e.GivenTime))
        {
            await IO.WriteLineAsync($"{e.Type}\t{e.Amount}\t{e.Category}\t{e.GivenTime:yy-MM-dd}\t{e.Description}");
        }
        IO.SubtractWriteLinePrefix();
        await IO.WriteLineAsync("Categories:", OutputType.Title);
        IO.AppendWriteLinePrefix();
        await IO.WriteLineAsync("Category\tAmount\tPercent");
        var full = Console.BufferWidth - 64 - cat.Max(c => c.Item1.Length);
        var total = cat[0].Item2;
        foreach (var (c,a) in cat)
        {
            var pct = a / total;
            var l = (int)(full * pct);
            await IO.WriteLineAsync(Suit.CreateContentArray(
                (c,null,null),
                ("\t", null, null),
                (a.ToString("0.00"),null,null),
                ("\t", null, null),
                (pct.ToString("0.00"),null,null),
                ("\t", null, null),
                (new string(' ',l), Color.White, Color.Red), 
                (new string(' ', full-l), Color.White, Color.Green)));
        }
        IO.SubtractWriteLinePrefix();
        await IO.WriteLineAsync("Times:", OutputType.Title);
        IO.AppendWriteLinePrefix();
        await IO.WriteLineAsync("Time\tAmount\tPercent");
        foreach (var (c, a) in time)
        {
            var pct = a / total;
            var l = (int)(full * pct);
            await IO.WriteLineAsync(Suit.CreateContentArray(
                (c, null, null),
                ("\t", null, null),
                (a.ToString("0.00"), null, null),
                ("\t", null, null),
                (pct.ToString("0.00"), null, null),
                ("\t", null, null),
                (new string(' ', l), Color.White, Color.Red),
                (new string(' ', full - l), Color.White, Color.Green)));
        }
        IO.SubtractWriteLinePrefix();
    }
}