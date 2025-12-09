using Xunit;
using HitRefresh.WebLedger.Services;
using HitRefresh.WebLedger.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebLedger.Tests
{
    /// <summary>
    /// 私有方法测试
    /// </summary>
    public class DirectLedgerManagerTests_PrivateMethods : IAsyncLifetime
    {
        private LedgerContext _context;
        private DirectLedgerManager _manager;
        private readonly string _databaseName;

        public DirectLedgerManagerTests_PrivateMethods()
        {
            _databaseName = $"TestDb_PrivateMethods_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<LedgerContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options;

            _context = new LedgerContext(options);
            var logger = new NullLogger<DirectLedgerManager>();
            _manager = new DirectLedgerManager(_context, logger);
        }

        public async Task InitializeAsync()
        {
            // 初始化基础数据
            var categories = new List<LedgerEntryCategory>
            {
                new() { Name = "工资", SuperCategoryName = null },
                new() { Name = "食品", SuperCategoryName = null },
                new() { Name = "主食", SuperCategoryName = "食品" },
                new() { Name = "零食", SuperCategoryName = "食品" },
                new() { Name = "交通", SuperCategoryName = null }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            _context.Dispose();
        }

        [Fact]
        public void GetFullCategories_EmptyDatabase_ReturnsEmptyDictionary()
        {
            // Arrange
            var emptyContextOptions = new DbContextOptionsBuilder<LedgerContext>()
                .UseInMemoryDatabase(databaseName: $"EmptyDb_{Guid.NewGuid()}")
                .Options;

            using var emptyContext = new LedgerContext(emptyContextOptions);
            var manager = new DirectLedgerManager(emptyContext, new NullLogger<DirectLedgerManager>());

            // Act - 使用反射调用私有方法
            var result = PrivateMethodInvoker.InvokePrivateMethod<Dictionary<string, HashSet<string>>>(
                manager, "GetFullCategories");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // 只包含 (Total)
            Assert.Contains("(Total)", result.Keys);
            Assert.Empty(result["(Total)"]);
        }

        [Fact]
        public void GetFullCategories_WithHierarchy_ReturnsCorrectMap()
        {
            // Act
            var result = PrivateMethodInvoker.InvokePrivateMethod<Dictionary<string, HashSet<string>>>(
                _manager, "GetFullCategories");

            // Assert
            Assert.NotNull(result);

            // 检查总集合
            Assert.Contains("(Total)", result.Keys);
            Assert.Equal(5, result["(Total)"].Count);

            // 检查食品分类包含主食和零食
            Assert.Contains("食品", result.Keys);
            var foodCategories = result["食品"];
            Assert.Contains("主食", foodCategories);
            Assert.Contains("零食", foodCategories);

            // 检查主食分类只包含自身
            Assert.Contains("主食", result.Keys);
            Assert.Single(result["主食"]);
            Assert.Contains("主食", result["主食"]);

            // 检查所有分类都包含自身
            var rootCategories = result.Where(kvp => kvp.Key != "(Total)" && !kvp.Value.Contains(kvp.Key));
            Assert.Empty(rootCategories);
        }

        [Fact]
        public void AutomationGeneratedViewName_Weekly_ReturnsCorrectFormat()
        {
            // Act
            var automation = new LedgerViewAutomation
            {
                TemplateName = "周报",
                Type = LedgerViewAutomationType.Weekly
            };

            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<string>(
                typeof(DirectLedgerManager),
                "AutomationGeneratedViewName",
                automation); // 注意：这里只传递一个参数

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("周报:", result);
            Assert.Contains("Week", result);

            var weekNumber = new System.Globalization.GregorianCalendar()
                .GetWeekOfYear(DateTime.Today,
                    System.Globalization.CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday);
            Assert.Contains($"Week #{weekNumber}", result);
        }

        [Fact]
        public void AutomationGeneratedViewName_Quarterly_ReturnsCorrectFormat()
        {
            // Act
            var automation = new LedgerViewAutomation
            {
                TemplateName = "季报",
                Type = LedgerViewAutomationType.Quarterly
            };

            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<string>(
                typeof(DirectLedgerManager),
                "AutomationGeneratedViewName",
                automation); // 注意：这里只传递一个参数

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("季报:", result);
            Assert.Contains("Quarter", result);

            var currentQuarter = (DateTime.Today.Month - 1) / 4;
            Assert.Contains($"Quarter #{currentQuarter}", result);
        }

        [Fact]
        public void AutomationGeneratedViewName_DefaultCase_ThrowsException()
        {
            // Arrange
            var automation = new LedgerViewAutomation
            {
                TemplateName = "测试",
                Type = (LedgerViewAutomationType)999 // 无效的枚举值
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                PrivateMethodInvoker.InvokePrivateStaticMethod<string>(
                    typeof(DirectLedgerManager),
                    "AutomationGeneratedViewName",
                    automation)
            );

            Assert.Contains("不支持的自动化类型", exception.Message);
        }

        [Fact]
        public void AutomationGeneratedViewStartTime_Weekly_ReturnsMonday()
        {
            // Act
            var automation = new LedgerViewAutomation
            {
                TemplateName = "测试",
                Type = LedgerViewAutomationType.Weekly
            };

            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<DateTime>(
                typeof(DirectLedgerManager),
                "AutomationGeneratedViewStartTime",
                automation);

            // Assert
            Assert.Equal(DayOfWeek.Monday, result.DayOfWeek);
            Assert.True(result <= DateTime.Today);
            Assert.True(result > DateTime.Today.AddDays(-7));
        }

        [Fact]
        public void AutomationGeneratedViewStartTime_Quarterly_ReturnsQuarterStart()
        {
            // Act
            var automation = new LedgerViewAutomation
            {
                TemplateName = "测试",
                Type = LedgerViewAutomationType.Quarterly
            };

            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<DateTime>(
                typeof(DirectLedgerManager),
                "AutomationGeneratedViewStartTime",
                automation);

            // Assert
            var expectedMonth = DateTime.Today.Month switch
            {
                >= 1 and < 4 => 1,
                >= 4 and < 7 => 4,
                >= 7 and < 10 => 7,
                _ => 10
            };

            Assert.Equal(expectedMonth, result.Month);
            Assert.Equal(1, result.Day);
            Assert.Equal(DateTime.Today.Year, result.Year);
        }

        [Fact]
        public void AutomationGeneratedViewStartTime_Yearly_ReturnsYearStart()
        {
            // Act
            var automation = new LedgerViewAutomation
            {
                TemplateName = "测试",
                Type = LedgerViewAutomationType.Yearly
            };

            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<DateTime>(
                typeof(DirectLedgerManager),
                "AutomationGeneratedViewStartTime",
                automation);

            // Assert
            Assert.Equal(1, result.Month);
            Assert.Equal(1, result.Day);
            Assert.Equal(DateTime.Today.Year, result.Year);
        }

        [Fact]
        public void AutomationGeneratedViewEndTime_Weekly_ReturnsNextMonday()
        {
            // Act
            var automation = new LedgerViewAutomation
            {
                TemplateName = "测试",
                Type = LedgerViewAutomationType.Weekly
            };

            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<DateTime>(
                typeof(DirectLedgerManager),
                "AutomationGeneratedViewEndTime",
                automation);

            // Assert
            var startTime = PrivateMethodInvoker.InvokePrivateStaticMethod<DateTime>(
                typeof(DirectLedgerManager),
                "AutomationGeneratedViewStartTime",
                automation);

            Assert.Equal(startTime.AddDays(7), result);
        }

        [Fact]
        public void AutomationGeneratedViewEndTime_Quarterly_ReturnsNextQuarterStart()
        {
            // Act
            var automation = new LedgerViewAutomation
            {
                TemplateName = "测试",
                Type = LedgerViewAutomationType.Quarterly
            };

            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<DateTime>(
                typeof(DirectLedgerManager),
                "AutomationGeneratedViewEndTime",
                automation);

            // 根据修复后的生产代码逻辑，季度结束时间应该是下一个季度的开始
            // 生产代码中季度结束时间的计算：
            // 1-3月 => 4月1日
            // 4-6月 => 7月1日
            // 7-9月 => 10月1日
            // 10-12月 => 明年1月1日

            var today = DateTime.Today;
            var expectedMonth = today.Month switch
            {
                >= 1 and <= 3 => 4,    // Q1 -> Q2 (4月)
                >= 4 and <= 6 => 7,    // Q2 -> Q3 (7月)
                >= 7 and <= 9 => 10,   // Q3 -> Q4 (10月)
                _ => 1                 // Q4 -> 明年Q1 (1月)
            };

            var expectedYear = expectedMonth == 1 ? today.Year + 1 : today.Year;

            Assert.Equal(expectedMonth, result.Month);
            Assert.Equal(1, result.Day);
            Assert.Equal(expectedYear, result.Year);
        }

        [Fact]
        public void SumByTime_SingleDay_ReturnsHourlyGrouping()
        {
            // Arrange
            var baseTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var entries = new List<LedgerEntry>
            {
                new() { Id = Guid.NewGuid(), Amount = -10m, GivenTime = baseTime.AddHours(2), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -20m, GivenTime = baseTime.AddHours(2).AddMinutes(30), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -15m, GivenTime = baseTime.AddHours(5), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -25m, GivenTime = baseTime.AddHours(10), CategoryName = "食品" }
            };

            // Act - 修复：传递一个包含数组的参数
            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<Dictionary<string, decimal>>(
                typeof(DirectLedgerManager),
                "SumByTime",
                new object[] { entries.ToArray() });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count); // 02:00, 05:00, 10:00
            Assert.Contains("02:00", result.Keys);
            Assert.Contains("05:00", result.Keys);
            Assert.Contains("10:00", result.Keys);
            Assert.Equal(30m, result["02:00"]); // 10 + 20
            Assert.Equal(15m, result["05:00"]);
            Assert.Equal(25m, result["10:00"]);
        }

        [Fact]
        public void SumByTime_FortyDays_ReturnsDailyGrouping()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var entries = new List<LedgerEntry>
            {
                new() { Id = Guid.NewGuid(), Amount = -10m, GivenTime = startTime, CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -20m, GivenTime = startTime.AddDays(20), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -30m, GivenTime = startTime.AddDays(35), CategoryName = "食品" }
            };

            // Act - 修复：传递一个包含数组的参数
            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<Dictionary<string, decimal>>(
                typeof(DirectLedgerManager),
                "SumByTime",
                new object[] { entries.ToArray() });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("0101", result.Keys); // 1月1日
            Assert.Contains("0121", result.Keys); // 1月21日
            Assert.Contains("0205", result.Keys); // 2月5日（35天后）
        }

        [Fact]
        public void SumByTime_OneYear_ReturnsMonthlyGrouping()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var entries = new List<LedgerEntry>
            {
                new() { Id = Guid.NewGuid(), Amount = -10m, GivenTime = startTime, CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -20m, GivenTime = startTime.AddMonths(3), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -30m, GivenTime = startTime.AddMonths(6), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -40m, GivenTime = startTime.AddMonths(9), CategoryName = "食品" }
            };

            // Act - 修复：传递一个包含数组的参数
            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<Dictionary<string, decimal>>(
                typeof(DirectLedgerManager),
                "SumByTime",
                new object[] { entries.ToArray() });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Contains("2301", result.Keys); // 2023年1月
            Assert.Contains("2304", result.Keys); // 2023年4月
            Assert.Contains("2307", result.Keys); // 2023年7月
            Assert.Contains("2310", result.Keys); // 2023年10月
        }

        [Fact]
        public void SumByTime_MultipleYears_ReturnsYearlyGrouping()
        {
            // Arrange
            var entries = new List<LedgerEntry>
            {
                new() { Id = Guid.NewGuid(), Amount = -10m, GivenTime = new DateTime(2020, 1, 1), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -20m, GivenTime = new DateTime(2021, 6, 1), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -30m, GivenTime = new DateTime(2022, 12, 1), CategoryName = "食品" },
                new() { Id = Guid.NewGuid(), Amount = -40m, GivenTime = new DateTime(2023, 7, 1), CategoryName = "食品" }
            };

            // Act - 修复：传递一个包含数组的参数
            var result = PrivateMethodInvoker.InvokePrivateStaticMethod<Dictionary<string, decimal>>(
                typeof(DirectLedgerManager),
                "SumByTime",
                new object[] { entries.ToArray() });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Contains("2020", result.Keys);
            Assert.Contains("2021", result.Keys);
            Assert.Contains("2022", result.Keys);
            Assert.Contains("2023", result.Keys);
        }
    }
}