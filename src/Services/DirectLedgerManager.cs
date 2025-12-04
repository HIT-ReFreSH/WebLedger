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
        // 修复：使用传统方式而不是 ExecuteDeleteAsync，因为 InMemory 数据库不支持
        var entry = await database.LedgerEntries.FirstOrDefaultAsync(e => e.Id == id);
        if (entry != null)
        {
            database.LedgerEntries.Remove(entry);
            await database.SaveChangesAsync();
        }
    }

    public async Task<string> Insert(Entry entry)
    {
        var type = await database.Types.Include(t => t.DefaultCategory)
            .FirstOrDefaultAsync(t => t.Name == entry.Type);
        var category = await database.Categories.FirstOrDefaultAsync(c => c.Name == entry.Category);
        if (type is null)
        {
            if (category is null)
                throw new InvalidOperationException($"Type '{entry.Type}' is not defined");
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

        // 获取所有分类
        var allCategories = database.Categories.ToList();

        // 如果没有分类，返回空字典
        if (!allCategories.Any())
        {
            catMap.Add("(Total)", new HashSet<string>());
            return catMap;
        }

        // 构建父分类到子分类的映射
        var childrenMap = new Dictionary<string, List<string>>();
        foreach (var category in allCategories)
        {
            var parent = category.SuperCategoryName ?? "(null)";
            if (!childrenMap.ContainsKey(parent))
            {
                childrenMap[parent] = new List<string>();
            }
            childrenMap[parent].Add(category.Name);
        }

        // 为每个分类构建包含自身及其所有子分类的集合
        foreach (var category in allCategories)
        {
            var categorySet = new HashSet<string>();

            // 使用栈进行深度优先遍历
            var stack = new Stack<string>();
            stack.Push(category.Name);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                // 避免循环引用
                if (categorySet.Contains(current))
                    continue;

                categorySet.Add(current);

                // 添加所有子分类
                if (childrenMap.TryGetValue(current, out var children))
                {
                    foreach (var child in children)
                    {
                        stack.Push(child);
                    }
                }
            }

            catMap[category.Name] = categorySet;
        }

        // 构建总集合
        foreach (var category in allCategories)
        {
            full.Add(category.Name);
        }

        catMap.Add("(Total)", full);

        return catMap;
    }

    public async Task<IList<RecordedEntry>> Select(SelectOption option)
    {
        IQueryable<LedgerEntry> query = database.LedgerEntries;

        if (option.Category != null)
        {
            // 获取所有需要包含的分类
            var allCategories = await database.Categories.ToListAsync();
            var categoriesToInclude = new HashSet<string>();

            // 使用队列进行广度优先搜索，避免无限循环
            var queue = new Queue<string>();
            queue.Enqueue(option.Category);

            // 限制最大搜索深度，避免无限循环
            int maxDepth = 100;
            int currentDepth = 0;
            var visited = new HashSet<string>();

            while (queue.Count > 0 && currentDepth < maxDepth)
            {
                int levelSize = queue.Count;
                for (int i = 0; i < levelSize; i++)
                {
                    var current = queue.Dequeue();

                    if (visited.Contains(current))
                        continue;

                    visited.Add(current);
                    categoriesToInclude.Add(current);

                    // 查找所有父分类是当前分类的子分类
                    var children = allCategories
                        .Where(c => c.SuperCategoryName == current)
                        .Select(c => c.Name)
                        .ToList();

                    foreach (var child in children)
                    {
                        if (!visited.Contains(child))
                        {
                            queue.Enqueue(child);
                        }
                    }
                }
                currentDepth++;
            }

            if (currentDepth >= maxDepth)
            {
                _logger.LogWarning($"分类层次搜索达到最大深度 {maxDepth}，可能有循环引用");
            }

            query = query.Where(e => categoriesToInclude.Contains(e.CategoryName));
        }

        return await query
            .Where(e => option.Direction == null || e.IsIncome == option.Direction)
            .Where(e => e.GivenTime >= option.StartTime && e.GivenTime <= option.EndTime)
            .Select(e => new RecordedEntry(e.Id, e.Amount, e.GivenTime, e.TypeName, e.CategoryName, e.Description))
            .ToListAsync();
    }

    public async Task<IList<Category>> GetAllCategories()
    {
        return await database.Categories.Select(c => new Category(c.Name, c.SuperCategoryName)).ToArrayAsync();
    }

    public async Task EnableViewAutomation(ViewAutomation automation)
    {
        if (await database.ViewAutomation.AnyAsync(a =>
            a.TemplateName == automation.TemplateName && a.Type == automation.Type))
            return;

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
            throw new InvalidOperationException($"View template '{automation.TemplateName}' is not defined");
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
        var today = DateTime.Today;
        var typeExpr = automation.Type switch
        {
            LedgerViewAutomationType.Daily => today.ToString("yyMMdd"),
            LedgerViewAutomationType.Weekly => $"({today.Year}) Week #{new GregorianCalendar().GetWeekOfYear(today, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}",
            LedgerViewAutomationType.Monthly => today.ToString("yyMM"),
            LedgerViewAutomationType.Quarterly => $"({today.Year}) Quarter #{((today.Month - 1) / 4)}",
            LedgerViewAutomationType.Yearly => today.ToString("yyyy"),
            _ => throw new ArgumentOutOfRangeException(nameof(automation.Type), $"不支持的自动化类型: {automation.Type}")
        };
        return $"{automation.TemplateName}:{typeExpr}";
    }

    private static DateTime AutomationGeneratedViewStartTime(LedgerViewAutomation automation)
    {
        var today = DateTime.Today;

        return automation.Type switch
        {
            LedgerViewAutomationType.Daily => today,
            LedgerViewAutomationType.Weekly => today.AddDays(-(today.DayOfWeek switch
            {
                DayOfWeek.Sunday => 6,
                var a => (int)a - 1,
            })),
            LedgerViewAutomationType.Monthly => new(today.Year, today.Month, 1),
            LedgerViewAutomationType.Quarterly => new(today.Year, today.Month switch
            {
                >= 1 and < 4 => 1,
                >= 4 and < 7 => 4,
                >= 7 and < 10 => 7,
                _ => 10
            }, 1),
            LedgerViewAutomationType.Yearly => new(today.Year, 1, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(automation.Type), $"不支持的自动化类型: {automation.Type}")
        };
    }

    private static DateTime AutomationGeneratedViewEndTime(LedgerViewAutomation automation)
    {
        var today = DateTime.Today;

        return automation.Type switch
        {
            LedgerViewAutomationType.Daily => today.AddDays(1),
            LedgerViewAutomationType.Weekly => today.AddDays(-(today.DayOfWeek switch
            {
                DayOfWeek.Sunday => 6,
                var a => (int)a - 1,
            })).AddDays(7),
            LedgerViewAutomationType.Monthly => new DateTime(today.Year, today.Month, 1).AddMonths(1),
            LedgerViewAutomationType.Quarterly => GetQuarterEndDate(today),
            LedgerViewAutomationType.Yearly => new(today.Year + 1, 1, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(automation.Type), $"不支持的自动化类型: {automation.Type}")
        };
    }

    private static DateTime GetQuarterEndDate(DateTime date)
    {
        var quarter = (date.Month - 1) / 3; // 0, 1, 2, 3
        var quarterStartMonth = quarter * 3 + 1; // 1, 4, 7, 10
        var quarterEndMonth = quarterStartMonth + 3;

        if (quarterEndMonth > 12)
        {
            return new DateTime(date.Year + 1, 1, 1);
        }
        else
        {
            return new DateTime(date.Year, quarterEndMonth, 1);
        }
    }

    private async Task CheckOutAutomation()
    {
        // 获取所有视图名称 - 修复：使用 ToListAsync() 而不是 ToHashSetAsync()
        var viewNames = await database.Views
            .Select(view => view.Name)
            .ToListAsync();
        var views = new HashSet<string>(viewNames);

        // 获取所有自动化配置 - 修复：先获取所有数据，然后在内存中分组和排序
        var allAutomations = await database.ViewAutomation
            .ToListAsync();

        // 在内存中分组和排序
        var automations = allAutomations
            .GroupBy(a => a.Type)
            .OrderBy(g => g.Key)
            .ToList();

        bool anyCreated = false;

        foreach (var group in automations)
        {
            bool groupCreated = false;

            foreach (var automation in group)
            {
                var viewName = AutomationGeneratedViewName(automation);

                // 如果视图已存在，跳过
                if (views.Contains(viewName))
                    continue;

                // 检查模板是否存在
                var templateExists = await database.ViewTemplates
                    .AnyAsync(t => t.Name == automation.TemplateName);

                if (!templateExists)
                {
                    _logger.LogWarning($"自动化配置引用了不存在的模板: {automation.TemplateName}");
                    continue;
                }

                // 创建视图
                var view = new LedgerView
                {
                    Name = viewName,
                    CreateTime = DateTime.Now,
                    StartTime = AutomationGeneratedViewStartTime(automation),
                    EndTime = AutomationGeneratedViewEndTime(automation),
                    TemplateName = automation.TemplateName
                };

                database.Views.Add(view);
                views.Add(viewName); // 更新本地集合，避免重复
                anyCreated = true;
                groupCreated = true;

                _logger.LogInformation($"创建自动化视图: {viewName}");
            }

            // 如果当前类型的自动化没有创建任何视图，则根据规则决定是否继续
            if (!groupCreated && group.Key == LedgerViewAutomationType.Weekly)
            {
                break;
            }
        }

        if (anyCreated)
        {
            try
            {
                await database.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"保存自动化视图时出错: {ex.Message}");
            }
        }
    }

    public async Task AddOrUpdateViewTemplate(ViewTemplate template)
    {
        var dbTemplate = await database.ViewTemplates.AsTracking().FirstOrDefaultAsync(c => c.Name == template.Name);

        if (dbTemplate is not null)
        {
            dbTemplate.IsIncome = template.IsIncome;
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
            throw new InvalidOperationException($"View template '{view.TemplateName}' is not defined");
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
            .OrderByDescending(v => v.CreateTime)
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
        if (dbTemplate == null) throw new InvalidOperationException($"View template '{name}' is not defined");
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
            e => e.Key, e => Math.Abs(e.Sum(x => x.Amount)));
    }

    public async Task<ViewQueryResult> Query(ViewQueryOption view)
    {
        var dbView = await database.Views.Include(v => v.Template)
            .FirstOrDefaultAsync(v => v.Name == view.ViewName);
        if (dbView == null) throw new InvalidOperationException($"View '{view.ViewName}' is not defined");

        // 使用修复后的 GetFullCategories 方法
        var catMap = GetFullCategories();
        var categories = dbView.Template.Categories.Split('|', StringSplitOptions.RemoveEmptyEntries);

        if (categories.Length == 0)
        {
            _logger.LogWarning($"视图模板 '{dbView.Template.Name}' 没有指定分类");
            return new(new List<Entry>(), new Dictionary<string, decimal>(), new Dictionary<string, decimal>());
        }

        var fullCat = new HashSet<string>();
        foreach (var cat in categories)
        {
            if (catMap.TryGetValue(cat, out var subCategories))
            {
                fullCat.UnionWith(subCategories);
            }
            else
            {
                _logger.LogWarning($"分类 '{cat}' 不存在于分类映射中");
                // 如果分类不存在，至少包含自身
                fullCat.Add(cat);
            }
        }

        var raws = await database.LedgerEntries.Where(
                e => e.GivenTime >= dbView.StartTime && e.GivenTime < dbView.EndTime)
            .Where(e => fullCat.Contains(e.CategoryName))
            .Where(e => dbView.Template.IsIncome == e.IsIncome)
            .ToArrayAsync();

        // ByCat - 修复金额计算
        var byCatOrigin = new Dictionary<string, decimal>();

        // 初始化所有相关分类的金额为0
        foreach (var cat in fullCat)
        {
            byCatOrigin[cat] = 0;
        }

        // 累加实际金额
        foreach (var entry in raws)
        {
            if (byCatOrigin.ContainsKey(entry.CategoryName))
            {
                byCatOrigin[entry.CategoryName] += entry.Amount;
            }
            else
            {
                byCatOrigin[entry.CategoryName] = entry.Amount;
            }
        }

        var byCatSum = new Dictionary<string, decimal>();

        // 计算每个指定分类及其子分类的金额总和（取绝对值）
        foreach (var category in categories)
        {
            if (catMap.TryGetValue(category, out var subCategories))
            {
                decimal sum = 0;
                foreach (var subCat in subCategories)
                {
                    sum += byCatOrigin.GetValueOrDefault(subCat, 0);
                }
                byCatSum[category] = Math.Abs(sum);
            }
            else
            {
                // 如果分类不在映射中，直接使用其金额
                byCatSum[category] = Math.Abs(byCatOrigin.GetValueOrDefault(category, 0));
            }
        }

        // 计算总金额
        var total = 0m;
        var processedCategories = new HashSet<string>();

        foreach (var category in categories)
        {
            if (catMap.TryGetValue(category, out var subCategories))
            {
                foreach (var subCat in subCategories)
                {
                    if (!processedCategories.Contains(subCat))
                    {
                        total += Math.Abs(byCatOrigin.GetValueOrDefault(subCat, 0));
                        processedCategories.Add(subCat);
                    }
                }
            }
            else
            {
                if (!processedCategories.Contains(category))
                {
                    total += Math.Abs(byCatOrigin.GetValueOrDefault(category, 0));
                    processedCategories.Add(category);
                }
            }
        }

        byCatSum.Add("(Total)", total);

        var byTime = SumByTime(raws);
        var takenRaws = raws
            .OrderByDescending(r => Math.Abs(r.Amount))
            .AsEnumerable();

        if (view.Limit >= 0)
            takenRaws = takenRaws.Take(view.Limit);

        return new(
            takenRaws.Select(e => new Entry(e.Amount, e.GivenTime, e.TypeName, e.CategoryName, e.Description)).ToList(),
            byCatSum,
            byTime);
    }
}