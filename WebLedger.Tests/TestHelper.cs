using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WebLedger.Tests
{
    /// <summary>
    /// 测试辅助工具类
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// 执行异步测试，并设置超时保护（增强版）
        /// </summary>
        public static async Task ExecuteWithTimeout(Func<Task> testAction, int timeoutSeconds = 30)
        {
            if (testAction == null)
                throw new ArgumentNullException(nameof(testAction));

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                // 包装测试任务，以便可以取消
                var testTask = Task.Run(async () =>
                {
                    await testAction();
                }, cts.Token);

                await testTask;
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                throw new TimeoutException($"测试执行时间超过 {timeoutSeconds} 秒，已中断");
            }
            catch (Exception ex)
            {
                // 如果测试中抛出了其他异常，重新抛出
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        /// <summary>
        /// 执行带返回值的异步测试，并设置超时保护
        /// </summary>
        public static async Task<T> ExecuteWithTimeout<T>(Func<Task<T>> testAction, int timeoutSeconds = 30)
        {
            if (testAction == null)
                throw new ArgumentNullException(nameof(testAction));

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                var testTask = Task.Run(async () =>
                {
                    return await testAction();
                }, cts.Token);

                return await testTask;
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                throw new TimeoutException($"测试执行时间超过 {timeoutSeconds} 秒，已中断");
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw; // 这行代码实际上不会执行，只是为了满足编译器
            }
        }

        /// <summary>
        /// 记录测试超时信息（用于调试）
        /// </summary>
        public static void LogTimeoutInfo(string testName, int timeoutSeconds)
        {
            Debug.WriteLine($"测试 '{testName}' 设置了 {timeoutSeconds} 秒超时保护");
        }

        /// <summary>
        /// 安全运行可能抛出异常的测试
        /// </summary>
        public static async Task<Exception> SafeRunAsync(Func<Task> action)
        {
            try
            {
                await action();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        /// 断言两个字典相等
        /// </summary>
        public static void AssertDictionariesEqual<TKey, TValue>(
            IDictionary<TKey, TValue> expected,
            IDictionary<TKey, TValue> actual,
            string message = "")
        {
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);

            foreach (var kvp in expected)
            {
                Assert.True(actual.ContainsKey(kvp.Key),
                    $"{message} 字典缺少键: {kvp.Key}");
                Assert.Equal(kvp.Value, actual[kvp.Key]);
            }
        }
    }
}