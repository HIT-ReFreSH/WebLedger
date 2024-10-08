﻿using System.Drawing;
using HitRefresh.WebLedger.Data;
using HitRefresh.WebLedger.Models;
using HitRefresh.WebLedger.Services;
using HitRefresh.MobileSuit;
using HitRefresh.MobileSuit.UI;
using HitRefresh.WebLedger.CLI.Services;

namespace HitRefresh.WebLedger.CLI;

[SuitInfo("WebLedger")]
public class Driver(IConfigManager config, ILedgerManager ledger, IIOHub IO, WebGuiHelper webGui)
{
    [SuitAlias("add")]
    [SuitInfo("Create an entry")]
    public async Task Insert()
    {
        var amount = decimal.Parse(await IO.ReadLineAsync("Input Amount") ?? "0");
        var type = await IO.ReadLineAsync("Input Type") ?? "";
        var timeExpr = await IO.ReadLineAsync("Input Time (yyMMdd)");

        var time = string.IsNullOrEmpty(timeExpr) ? DateTime.Now :
            new DateTime(int.Parse(timeExpr[..2]) + 2000, int.Parse(timeExpr[2..4]), int.Parse(timeExpr[4..]));
        var categories = await ledger.GetAllCategories();
        var category = IO.CuiSelectItemFrom("Input Category", new[] { "(auto)" }.Concat(categories.Select(c => c.Name)).ToArray());
        category = category.Replace("(auto)", "");
        var desc = await IO.ReadLineAsync("Input Description") ?? "";
        var entry = new Entry(amount, time, type, category, desc);
        var id = await ledger.Insert(entry);
        await IO.WriteLineAsync($"Added: {id}");
    }
    [SuitAlias("cat")]
    [SuitInfo("Create category: cat <catname>[ <super-cat-name>]")]
    public async Task AddCategory(string catname, string? supcat = null)
    {
        await ledger.AddOrUpdateCategory(new(catname, supcat));
    }
    [SuitAlias("del-cat")]
    [SuitInfo("Delete category.")]
    public async Task DeleteCategory(string catname)
    {
        await ledger.RemoveCategory(catname);
    }
    [SuitAlias("grant")]
    [SuitInfo("Create an access: grant <access-name>; store the secret token!")]
    public async Task<string> GrantAccess(string access)
    {
        return await config.AddAccess(access);
    }
    [SuitAlias("ungrant")]
    [SuitInfo("Remove an access: ungrant <access-name>")]
    public async Task CancelAccess(string access)
    {
        await config.RemoveAccess(access);
    }
    [SuitAlias("ls-acc")]
    [SuitInfo("List all access & secrets")]
    public async Task GetAccess()
    {
        var access = await config.GetAllAccess();
        foreach (var a in access)
        {
            await IO.WriteLineAsync($"{a.Name}\t{a.Key}");
        }
    }
    [SuitAlias("ls-all")]
    [SuitInfo("List all ledger entries")]
    public async Task GetAll()
    {
        var entries = await ledger.Select(new(DateTime.MinValue, DateTime.Now, null, null));
        await IO.WriteLineAsync("Type\tAmount\tCategory\tTime\tDescription");
        foreach (var e in entries.OrderBy(e => e.GivenTime))
        {
            await IO.WriteLineAsync($"{e.Type}\t{e.Amount}\t{e.Category}\t{e.GivenTime:yy-MM-dd}\t{e.Description}");
        }
    }

    [SuitAlias("auto")]
    [SuitInfo("Create a view template automation")]
    public async Task EnableViewAutomation(string type)
    {
        var tmpls = await ledger.GetAllViewTemplateNames();
        var tmpl = IO.CuiSelectItemFrom("Select Template", tmpls.ToArray());
        await ledger.EnableViewAutomation(new(type.ToLower()[0] switch
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
    [SuitInfo("Remove a view template automation")]
    public async Task DisableViewAutomation()
    {
        var atms = await ledger.GetAllViewAutomation();

        var atm = IO.CuiSelectItemFrom("Select Automation", (new[] { new ViewAutomation(0, "(cancel)") }.Concat(atms)).ToArray());
        if (atm.TemplateName != "(cancel)")
            await ledger.DisableViewAutomation(atm);
    }
    [SuitAlias("tmpl")]
    [SuitInfo("Create a view template")]
    public async Task AddViewTemplate()
    {
        var name = await IO.ReadLineAsync("Input template name");
        if (name is null) return;
        var isIncome =  IO.CuiYesNo("Is Income?", false);
        var cats = await ledger.GetAllCategories();
        var selected = IO.CuiSelectItemsFrom("Select categories", cats.Select(c=>c.Name).ToArray()).ToArray();
        await ledger.AddOrUpdateViewTemplate(new(name, selected, isIncome));

    }
    [SuitAlias("ed-tmpl")]
    [SuitInfo("Edit a view template")]
    public async Task UpdateViewTemplate()
    {
        var tmpls = await ledger.GetAllViewTemplateNames();
        var tmpl = IO.CuiSelectItemFrom("Select Template", tmpls.ToArray());
        var origin = await ledger.GetViewTemplate(tmpl);
        var isIncome = IO.CuiYesNo("Is Income?", origin.IsIncome);
        var cats = (await ledger.GetAllCategories());
        var originSelected = origin.Categories.ToHashSet();
        var selected = IO.CuiSelectItemsFrom("Select categories", cats.Select(c => c.Name).ToArray(),
            c => originSelected.Contains(c) ? $"+{c}" : $"-{c}").ToArray();
        await ledger.AddOrUpdateViewTemplate(new(tmpl, selected, isIncome));
    }
    [SuitAlias("rm-tmpl")]
    [SuitInfo("Remove a view template")]
    public async Task RemoveViewTemplate(string template)
    {
        if (string.IsNullOrEmpty(template)) return;
        await ledger.RemoveViewTemplate(template);
    }
    [SuitAlias("ad-view")]
    [SuitInfo("Add a view")]
    public async Task AddView(string name)
    {
        var tmpls = await ledger.GetAllViewTemplateNames();
        var tmpl = IO.CuiSelectItemFrom("Select Template", tmpls.ToArray());

        var timeExpr = await IO.ReadLineAsync("Input Start Time (yyMMdd, today default)");

        var startTime = string.IsNullOrEmpty(timeExpr) ? DateTime.Today :
            new DateTime(int.Parse(timeExpr[..2]) + 2000, int.Parse(timeExpr[2..4]), int.Parse(timeExpr[4..]));
        timeExpr = await IO.ReadLineAsync("Input End Time (yyMMdd,tomorrow default)");

        var endTime = string.IsNullOrEmpty(timeExpr) ? DateTime.Today.AddDays(1) :
            new DateTime(int.Parse(timeExpr[..2]) + 2000, int.Parse(timeExpr[2..4]), int.Parse(timeExpr[4..]));
        await ledger.AddView(new(tmpl,startTime,endTime,tmpl));
    }
    [SuitAlias("rm-view")]
    [SuitInfo("Remove a view")]
    public async Task RemoveView()
    {
        var views = await ledger.GetAllViewNames();
        var view = IO.CuiSelectItemFrom("Select View", views.ToArray());
        await ledger.RemoveView(view);
    }
    [SuitAlias("ls-cat")]
    [SuitInfo("List all categories")]
    public async Task GetAllCategories()
    {
        var views = await ledger.GetAllCategories();
        foreach (var view in views)
        {
            await IO.WriteLineAsync(view.Name);
        }
    }
    [SuitAlias("ls-view")]
    [SuitInfo("List all views")]
    public async Task GetAllViewNames()
    {
        var views = await ledger.GetAllViewNames();
        foreach (var view in views)
        {
            await IO.WriteLineAsync(view);
        }
    }

    [SuitAlias("ls-tmpl")]
    [SuitInfo("List all view templates")]
    public async Task GetAllViewTemplateNames()
    {
        var views = await ledger.GetAllViewTemplateNames();
        foreach (var view in views)
        {
            await IO.WriteLineAsync(view);
        }
    }

    [SuitAlias("dt-tmpl")]
    [SuitInfo("Check view template details")]
    public async Task GetViewTemplate(string name)
    {
        var tmpl=await ledger.GetViewTemplate(name);
        await IO.WriteLineAsync($"Direction: {(tmpl.IsIncome?"Income":"Outcome")}");
        await IO.WriteLineAsync($"Categories: {(string.Join(", ",tmpl.Categories))}");
    }

    [SuitAlias("ls-auto")]
    [SuitInfo("View all automation views")]
    public async Task GetAllViewAutomation()
    {
        var views = await ledger.GetAllViewAutomation();
        await IO.WriteLineAsync("Type\tTemplate");
        foreach (var view in views)
        {
            await IO.WriteLineAsync($"{Enum.GetName( view.Type)}\t{view.TemplateName}");
        }
    }
    [SuitAlias("r")]
    [SuitInfo("Refund/remove ledger entry.")]
    public async Task Refund()
    {
        var cats = await ledger.GetAllCategories();
        var selected = IO.CuiSelectItemFrom("Select category", cats.Select(c => c.Name).ToArray());
        var timeExpr = await IO.ReadLineAsync("Input Time (yyMMdd)");

        var time = string.IsNullOrEmpty(timeExpr) ? DateTime.Today :
            new DateTime(int.Parse(timeExpr[..2]) + 2000, int.Parse(timeExpr[2..4]), int.Parse(timeExpr[4..]));
        var remote = await ledger.Select(new(time, time.AddDays(1), null, selected));
        if (remote.Count == 0)
        {
            await IO.WriteLineAsync("No Entry Found.");
            return;
        }
        var toRefund = IO.CuiSelectItemFrom("Select entry to refund", remote.ToArray(),
            e => $"{e.Type}\t{e.Amount}\t{e.Category}\t{e.GivenTime}\t{e.Description}");
        await ledger.Remove(toRefund.Id);
    }
    [SuitAlias("gq")]
    [SuitInfo("Display view graphically")]
    public async Task QueryGraphical()
    {
        var views = await ledger.GetAllViewNames();
        var view = IO.CuiSelectItemFrom("Select View", views.ToArray());
        await webGui.CachedQueryGraphical(new(view, 10));
    }
    [SuitAlias("q")]
    [SuitInfo("Display view")]
    public async Task Query()
    {
        var views = await ledger.GetAllViewNames();
        var view = IO.CuiSelectItemFrom("Select View", views.ToArray());
        var (raw,catD,timeD)= await ledger.Query(new(view, 10));
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
                ((100 * pct).ToString("0.00"),null,null),
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
                ((100*pct).ToString("0.00"), null, null),
                ("\t", null, null),
                (new string(' ', l), Color.White, Color.Red),
                (new string(' ', full - l), Color.White, Color.Green)));
        }
        IO.SubtractWriteLinePrefix();
    }
}