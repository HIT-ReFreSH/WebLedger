using HitReFreSH.WebLedger.Data;
using HitReFreSH.WebLedger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HitReFreSH.WebLedger.Services;

public class DirectLedgerManager : ILedgerManager
{
    private readonly LedgerContext _database;
    private readonly ILogger<DirectLedgerManager> _logger;

    public DirectLedgerManager(LedgerContext database, ILogger<DirectLedgerManager> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<string> Insert(Entry entry)
    {
        var type = await _database.Types.Include(t=>t.DefaultCategory)
            .FirstOrDefaultAsync(t => t.Name == entry.Type);
        var category = await _database.Categories.FirstOrDefaultAsync(c => c.Name == entry.Category);
        if (type is null)
        {
            if (category is null)
                throw new TypeUndefinedException(entry.Type);
            type = new LedgerEntryType
            {
                Name= entry.Type,
                DefaultCategory = category,
                DefaultIsIncome = entry.Amount > 0
            };
            await _database.Types.AddAsync(type);
            await _database.SaveChangesAsync();
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
        await _database.LedgerEntries.AddAsync(dbEntry);
        await _database.SaveChangesAsync();
        return dbEntry.Id.ToString();
    }

    public async Task AddOrUpdateCategory(Category category)
    {
        var dbCategory = new LedgerEntryCategory { Name = category.Name, SuperCategoryName = category.SuperCategory };
        if (await _database.Categories.AnyAsync(c => c.Name == category.Name))
            _database.Categories.Update(dbCategory);
        else
            await _database.Categories.AddAsync(dbCategory);

        await _database.SaveChangesAsync();
    }

    public async Task RemoveCategory(string category)
    {
        var dbCategory = new LedgerEntryCategory { Name = category };
        _database.Categories.Remove(dbCategory);

        await _database.SaveChangesAsync();
    }

    public async Task<IList<Entry>> Select(SelectOption option)
    {
        IQueryable<LedgerEntry> query = _database.LedgerEntries;
        if (option.Category != null)
        {
            HashSet<string> categories = new() { option.Category };
            for (;;)
            {
                var newCategories =
                    _database.Categories.Where(c => categories.Contains(c.SuperCategoryName))
                        .Select(c => c.Name).ToHashSet();
                if (newCategories.SetEquals(categories)) break;
                categories = newCategories;
            }

            query = query.Where(e => categories.Contains(e.CategoryName));
        }

        return await query
            .Where(e => option.Direction == null || e.IsIncome == option.Direction)
            .Where(e => e.GivenTime >= option.StartTime && e.GivenTime <= option.EndTime)
            .Select(e => new Entry(e.Amount, e.GivenTime, e.TypeName, e.CategoryName, e.Description)).ToListAsync();
    }

    public async Task<IList<Category>> GetAllCategories()
    {
        return await _database.Categories.Select(c => new Category(c.Name, c.SuperCategoryName)).ToArrayAsync();
    }
}