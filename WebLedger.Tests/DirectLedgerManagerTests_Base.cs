using Xunit;
using Microsoft.EntityFrameworkCore;
using HitRefresh.WebLedger.Data;
using HitRefresh.WebLedger.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics; // 添加这行

namespace WebLedger.Tests
{
    public abstract class DirectLedgerManagerTests_Base : IAsyncLifetime
    {
        protected LedgerContext Context;
        protected DirectLedgerManager Manager;
        protected readonly ILogger<DirectLedgerManager> Logger;

        private readonly string _databaseName;

        protected DirectLedgerManagerTests_Base()
        {
            _databaseName = $"TestDb_{GetType().Name}_{Guid.NewGuid()}";

            var options = new DbContextOptionsBuilder<LedgerContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options;

            Context = new LedgerContext(options);
            Logger = new NullLogger<DirectLedgerManager>();
            Manager = new DirectLedgerManager(Context, Logger);
        }

        public async Task InitializeAsync()
        {
            // 初始化基础数据
            await InitializeBaseData();
        }

        public async Task DisposeAsync()
        {
            await Context.Database.EnsureDeletedAsync();
            Context.Dispose();
        }

        private async Task InitializeBaseData()
        {
            // 创建基础分类
            var categories = new List<LedgerEntryCategory>
            {
                new() { Name = "工资", SuperCategoryName = null },
                new() { Name = "食品", SuperCategoryName = null },
                new() { Name = "交通", SuperCategoryName = null },
                new() { Name = "娱乐", SuperCategoryName = null },
                new() { Name = "购物", SuperCategoryName = null },
                new() { Name = "投资", SuperCategoryName = null },
                new() { Name = "医疗", SuperCategoryName = null }
            };

            await Context.Categories.AddRangeAsync(categories);
            await Context.SaveChangesAsync();
        }

        // 获取或创建分类，如果已存在则返回
        protected async Task<LedgerEntryCategory> GetOrCreateCategory(string name, string superCategoryName = null)
        {
            // 先检查 ChangeTracker 中是否有已经跟踪的实体
            var trackedCategory = Context.ChangeTracker.Entries<LedgerEntryCategory>()
                .FirstOrDefault(e => e.Entity.Name == name)?.Entity;

            if (trackedCategory != null)
            {
                // 如果已经跟踪，直接返回
                return trackedCategory;
            }

            // 否则从数据库查询
            var category = await Context.Categories
                .FirstOrDefaultAsync(c => c.Name == name);

            if (category == null)
            {
                category = new LedgerEntryCategory
                {
                    Name = name,
                    SuperCategoryName = superCategoryName
                };
                await Context.Categories.AddAsync(category);
                await Context.SaveChangesAsync();
            }
            else if (superCategoryName != null && category.SuperCategoryName != superCategoryName)
            {
                // 更新分类
                category.SuperCategoryName = superCategoryName;
                await Context.SaveChangesAsync();
            }

            return category;
        }

        protected async Task<LedgerEntryType> CreateTestType(string name, string categoryName, bool isIncome = false)
        {
            var category = await GetOrCreateCategory(categoryName);
            var type = new LedgerEntryType
            {
                Name = name,
                DefaultCategory = category,
                DefaultIsIncome = isIncome
            };

            await Context.Types.AddAsync(type);
            await Context.SaveChangesAsync();
            return type;
        }

        protected async Task<LedgerEntry> CreateTestEntry(
            decimal amount,
            DateTime givenTime,
            string typeName,
            string categoryName,
            string description = "")
        {
            var category = await GetOrCreateCategory(categoryName);
            var type = await Context.Types.FirstOrDefaultAsync(t => t.Name == typeName);

            if (type == null)
            {
                type = new LedgerEntryType
                {
                    Name = typeName,
                    DefaultCategory = category,
                    DefaultIsIncome = amount > 0
                };
                await Context.Types.AddAsync(type);
                await Context.SaveChangesAsync();
            }

            var entry = new LedgerEntry
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                GivenTime = givenTime,
                Description = description,
                IsIncome = amount > 0,
                Category = category,
                Type = type,
                CreateTime = DateTime.Now
            };

            await Context.LedgerEntries.AddAsync(entry);
            await Context.SaveChangesAsync();
            return entry;
        }

        // 清除 ChangeTracker，避免实体跟踪冲突
        protected void ClearChangeTracker()
        {
            Context.ChangeTracker.Clear();
        }

        // 辅助方法：安全删除分类
        protected async Task RemoveCategorySafely(string categoryName)
        {
            try
            {
                var category = await Context.Categories
                    .FirstOrDefaultAsync(c => c.Name == categoryName);

                if (category != null)
                {
                    Context.Categories.Remove(category);
                    await Context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"安全删除分类 '{categoryName}' 失败: {ex.Message}");
            }
        }
    }
}