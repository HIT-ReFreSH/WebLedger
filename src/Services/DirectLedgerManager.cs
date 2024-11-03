using HitRefresh.WebLedger.Data;
using HitRefresh.WebLedger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace HitRefresh.WebLedger.Services;

public class DirectLedgerManager(LedgerContext database, ILogger<DirectLedgerManager> logger)
    : ILedgerManager
{
    private readonly ILogger<DirectLedgerManager> _logger = logger;

    public async Task Remove(Guid id)
    {
        await database.LedgerEntries.Where(e => e.Id == id).ExecuteDeleteAsync();
    }
    public async Task<string> Insert(Entry entry)
    {
        var type = await database.Types.Include(t => t.DefaultCategory)
            .FirstOrDefaultAsync(t => t.Name == entry.Type);
        var category = await database.Categories.FirstOrDefaultAsync(c => c.Name == entry.Category);
        if (type is null)
        {
            if (category is null)
                throw new TypeUndefinedException(entry.Type);
            type = new LedgerEntryType
            {
                Name = entry.Type,
                DefaultCategory = category,
                DefaultIsIncome = entry.Amount > 0
            };
            await database.Types.AddAsync(type);
            await database.SaveChangesAsync();
        }

        category ??= type.DefaultCategory;
        var dbEntry = new LedgerEntry
        {
            GivenTime = entry.GivenTime,
            CreateTime = DateTime.Now,
            Description = entry.Description ?? "",
            IsIncome = entry.Amount > 0,
            Amount = entry.Amount,
            Category = category,
            Type = type
        };
        await database.LedgerEntries.AddAsync(dbEntry);
        await database.SaveChangesAsync();
        return dbEntry.Id.ToString();
    }

    public async Task AddOrUpdateCategory(Category category)
    {
        var dbCategory = await database.Categories.AsTracking().FirstOrDefaultAsync(c => c.Name == category.Name);
        if (dbCategory is not null)
            dbCategory.SuperCategoryName = category.SuperCategory;
        else
            await database.Categories.AddAsync(new LedgerEntryCategory { Name = category.Name, SuperCategoryName = category.SuperCategory });

        await database.SaveChangesAsync();
    }

    public async Task RemoveCategory(string category)
    {
        var dbCategory = new LedgerEntryCategory { Name = category };
        database.Categories.Remove(dbCategory);

        await database.SaveChangesAsync();
    }

    private Dictionary<string, HashSet<string>> GetFullCategories()
    {
        var catMap = new Dictionary<string, HashSet<string>>();
        var full = new HashSet<string>();
        var currentTree = new Stack<HashSet<string>>();
        // Empty bottom set
        currentTree.Push(new());
        currentTree.Push(full);
        var next = new Stack<HashSet<string?>>();
        next.Push(new() { null });
        while (next.Any())
        {
            var currentLayer = next.Peek();
            if (!currentLayer.Any())
            {
                next.Pop();
                currentTree.Pop();
                continue;
            }
            var currentNode = currentLayer.First();
            currentLayer.Remove(currentNode);
            if (currentNode != null)
            {
                currentTree.Push(new());
                catMap[currentNode] = currentTree.Peek();
                foreach (var s in currentTree)
                {
                    s.Add(currentNode);
                }

            }
            var currentSubCats = database.Categories
                .Where(c => c.SuperCategoryName == currentNode)
                .Select(c => c.Name)
                .Cast<string?>()
                .ToHashSet();


            next.Push(currentSubCats);

        }

        catMap.Add("(Total)", full);
        return catMap;
    }
    public async Task<IList<RecordedEntry>> Select(SelectOption option)
    {
        IQueryable<LedgerEntry> query = database.LedgerEntries;

        if (option.Category != null)
        {
            HashSet<string> categories = new() { option.Category };
            for (; ; )
            {
#pragma warning disable CS8604
                var newCategories =
                    database.Categories.Where(c => categories.Contains(c.SuperCategoryName))
                        .Select(c => c.Name).ToHashSet();
#pragma warning restore CS8604
                if (newCategories.Count==0) break;
                categories.UnionWith( newCategories);
            }

            query = query.Where(e => categories.Contains(e.CategoryName));
        }

        return await query
            .Where(e => option.Direction == null || e.IsIncome == option.Direction)
            .Where(e => e.GivenTime >= option.StartTime && e.GivenTime <= option.EndTime)
            .Select(e => new RecordedEntry(e.Id,e.Amount, e.GivenTime, e.TypeName, e.CategoryName, e.Description)).ToListAsync();
    }

    public async Task<IList<Category>> GetAllCategories()
    {
        return await database.Categories.Select(c => new Category(c.Name, c.SuperCategoryName)).ToArrayAsync();
    }

    public async Task EnableViewAutomation(ViewAutomation automation)
    {

        if (await database.ViewAutomation.AnyAsync(a =>
            a.TemplateName == automation.TemplateName && a.Type == automation.Type)) return;
        if (await database.ViewTemplates.AnyAsync(t => t.Name == automation.TemplateName))
        {
            await database.ViewAutomation.AddAsync(new()
            {
                TemplateName = automation.TemplateName,
                Type = automation.Type
            });
            await database.SaveChangesAsync();
        }
        else
        {
            throw new ViewTemplateUndefinedException(automation.TemplateName);
        }
    }

    public async Task DisableViewAutomation(ViewAutomation automation)
    {
        if (await database.ViewAutomation.AnyAsync(a =>
                a.TemplateName == automation.TemplateName && a.Type == automation.Type))
        {
            database.ViewAutomation.Remove(new()
            {
                TemplateName = automation.TemplateName,
                Type = automation.Type
            });
            await database.SaveChangesAsync();
        }
    }

    private static string AutomationGeneratedViewName(LedgerViewAutomation automation)
    {
        var typeExpr = automation.Type switch
        {
            LedgerViewAutomationType.Daily => DateTime.Today.ToString("yyMMdd"),
            LedgerViewAutomationType.Weekly => $"({DateTime.Today.Year}) Week #{new GregorianCalendar().GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}",
            LedgerViewAutomationType.Monthly => DateTime.Today.ToString("yyMM"),
            LedgerViewAutomationType.Quarterly => $"({DateTime.Today.Year}) Quarter #{((DateTime.Today.Month - 1) / 4)}",
            LedgerViewAutomationType.Yearly => DateTime.Today.ToString("yyyy"),
            _ => throw new ArgumentOutOfRangeException()
        };
        return $"{automation.TemplateName}:{typeExpr}";
    }
    private static DateTime AutomationGeneratedViewStartTime(LedgerViewAutomation automation)
    {
        return automation.Type switch
        {
            LedgerViewAutomationType.Daily => DateTime.Today,
            LedgerViewAutomationType.Weekly => DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek switch
            {
                DayOfWeek.Sunday => 6,
                var a => (int)a - 1,
            })),
            LedgerViewAutomationType.Monthly => new(DateTime.Today.Year, DateTime.Today.Month, 1),
            LedgerViewAutomationType.Quarterly => new(DateTime.Today.Year, DateTime.Today.Month switch
            {
                >= 1 and < 4 => 1,
                >= 4 and < 7 => 4,
                >= 7 and < 10 => 7,
                _ => 10
            }, 1),
            LedgerViewAutomationType.Yearly => new(DateTime.Today.Year, 1, 1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    private static DateTime AutomationGeneratedViewEndTime(LedgerViewAutomation automation)
    {
        return automation.Type switch
        {
            LedgerViewAutomationType.Daily => DateTime.Today.AddDays(1),
            LedgerViewAutomationType.Weekly => DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek switch
            {
                DayOfWeek.Sunday => 6,
                var a => (int)a - 1,
            })).AddDays(7),
            LedgerViewAutomationType.Monthly => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(DateTime.Today.Month switch
            {
                1 or 3 or 5 or 7 or 8 or 10 or 12 => 31,
                2 => 28,
                _ => 30
            }),
            LedgerViewAutomationType.Quarterly => new(DateTime.Today.Year, DateTime.Today.Month switch
            {
                >= 1 and < 4 => 4,
                >= 4 and < 7 => 7,
                >= 7 and < 10 => 10,
                _ => 1
            }, 1),
            LedgerViewAutomationType.Yearly => new(DateTime.Today.Year + 1, 1, 1),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="automation">Whether a new view is generated.</param>
    /// <returns></returns>
    private bool CheckOutAutomation(LedgerViewAutomation automation, IReadOnlySet<string> views)
    {
        var viewName = AutomationGeneratedViewName(automation);
        if (views.Contains(viewName)) { return (false); }

        var view = new LedgerView
        {
            Name = viewName,
            CreateTime = DateTime.Now,
            StartTime = AutomationGeneratedViewStartTime(automation),
            EndTime = AutomationGeneratedViewEndTime(automation),
            TemplateName = automation.TemplateName
        };
        database.Views.Add(view);

        return (true);
    }

    private async Task CheckOutAutomation()
    {
        var views = database.Views.Select(view => view.Name).ToHashSet();
        var automation = database.ViewAutomation.AsEnumerable()
            .GroupBy(a => a.Type)
            .OrderBy(g => g.Key);
        foreach (var g in automation)
        {
            var goNext = g.Any(a => CheckOutAutomation(a, views));
            // Shortcut, The Automation with larger type index will be Unexecuted if and only if
            // the smaller ones are all Unexecuted, except Weekly ones.
            if (!goNext && g.Key == LedgerViewAutomationType.Weekly) break;
        }
        await database.SaveChangesAsync();
    }
    public async Task AddOrUpdateViewTemplate(ViewTemplate template)
    {
        var dbTemplate = await database.ViewTemplates.AsTracking().FirstOrDefaultAsync(c => c.Name == template.Name);

        if (dbTemplate is not null)
        {
            dbTemplate.IsIncome=template.IsIncome;
            dbTemplate.Categories = string.Join('|', template.Categories);
        }
        else
            await database.ViewTemplates.AddAsync(new()
            {
                Name = template.Name,
                IsIncome = template.IsIncome,
                Categories = string.Join('|', template.Categories),
            });

        await database.SaveChangesAsync();
    }

    public async Task RemoveViewTemplate(string template)
    {
        database.ViewTemplates.Remove(new() { Name = template });

        await database.SaveChangesAsync();
    }

    public async Task AddView(View view)
    {
        if (await database.Views.AnyAsync(a =>
                a.Name == view.Name)) return;
        if (await database.ViewTemplates.AnyAsync(t => t.Name == view.TemplateName))
        {
            await database.Views.AddAsync(new()
            {
                TemplateName = view.TemplateName,
                CreateTime = DateTime.Now,
                EndTime = view.EndTime,
                StartTime = view.StartTime,
                Name = view.Name,
            });
            await database.SaveChangesAsync();
        }
        else
        {
            throw new ViewTemplateUndefinedException(view.TemplateName);
        }
    }

    public async Task RemoveView(string view)
    {
        database.Views.Remove(new() { Name = view });

        await database.SaveChangesAsync();
    }

    public async Task<IList<string>> GetAllViewNames()
    {
        await CheckOutAutomation();
        return await database.Views
            .OrderByDescending(v=>v.CreateTime)
            .Select(v => v.Name).ToListAsync();
    }

    public async Task<IList<string>> GetAllViewTemplateNames()
    {
        return await database.ViewTemplates.Select(v => v.Name)
            .ToListAsync();
    }

    public async Task<ViewTemplate> GetViewTemplate(string name)
    {
        var dbTemplate = await database.ViewTemplates.FirstOrDefaultAsync(v => v.Name == name);
        if (dbTemplate == null) throw new ViewTemplateUndefinedException(name);
        return new(dbTemplate.Name, dbTemplate.Categories.Split('|'), dbTemplate.IsIncome);
    }


    public async Task<IList<ViewAutomation>> GetAllViewAutomation()
    {
        return await database.ViewAutomation
            .Select(v => new ViewAutomation(v.Type, v.TemplateName))
            .ToListAsync();
    }

    private static Dictionary<string, decimal> SumByTime(LedgerEntry[] entries)
    {
        if (!entries.Any()) return new();
        var maxLength = entries.Max(e => e.GivenTime) - entries.Min(e => e.GivenTime);
        Func<LedgerEntry, string> groupBy;
        if (maxLength <= TimeSpan.FromDays(1))
        {
            groupBy = e => $"{e.GivenTime.Hour:00}:00";
        }
        else if (maxLength <= TimeSpan.FromDays(40))
        {
            groupBy = e => e.GivenTime.ToString("MMdd");
        }
        else if (maxLength <= TimeSpan.FromDays(366))
        {
            groupBy = e => e.GivenTime.ToString("yyMM");
        }
        else
        {
            groupBy = e => e.GivenTime.ToString("yyyy");
        }
        return entries.GroupBy(groupBy).ToDictionary(
            e=>e.Key,e=> Math.Abs(e.Sum(x => x.Amount)));

    }
    public async Task<ViewQueryResult> Query(ViewQueryOption view)
    {
        var dbView = await database.Views.Include(v => v.Template)
            .FirstOrDefaultAsync(v => v.Name == view.ViewName);
        if (dbView == null) throw new ViewUndefinedException(view.ViewName);
        var catMap = GetFullCategories();
        var categories = dbView.Template.Categories.Split('|');
        var fullCat = new HashSet<string>();
        foreach (var cat in categories.Select(c => catMap[c]))
        {
            fullCat.UnionWith(cat);
        }

        var raws = await database.LedgerEntries.Where(
                e => e.GivenTime >= dbView.StartTime && e.GivenTime < dbView.EndTime)
            .Where(e => fullCat.Contains(e.CategoryName))
            .Where(e => dbView.Template.IsIncome == e.IsIncome)
            .ToArrayAsync();
        // ByCat
        var byCatOrigin =
            raws.GroupBy(r => r.CategoryName)
                .ToDictionary(r => r.Key,
                    r => r.Sum(e => e.Amount));

        var byCatSum =
                categories.ToDictionary(c => c, c => Math.Abs(catMap[c].Sum(m => byCatOrigin.GetValueOrDefault(m))))
            ;
        var total = Math.Abs(categories.SelectMany(c => catMap[c]).Distinct()
            .Sum(m => byCatOrigin.GetValueOrDefault(m)));
        byCatSum.Add("(Total)", total);

        var byTime = SumByTime(raws);
        var takenRaws =
            raws.OrderByDescending(r => Math.Abs(r.Amount))
                .AsEnumerable();
        if (view.Limit >= 0) takenRaws = takenRaws.Take(view.Limit);

        return new(takenRaws.Select(e => new Entry(e.Amount, e.GivenTime, e.TypeName, e.CategoryName, e.Description))
            .ToList(), byCatSum, byTime);

    }
}