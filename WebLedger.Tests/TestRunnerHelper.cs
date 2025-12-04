using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace WebLedger.Tests
{
    public static class TestRunnerHelper
    {
        /// <summary>
        /// 运行测试并设置超时，防止测试卡死
        /// </summary>
        public static async Task RunTestWithTimeout(Func<Task> testAction, int timeoutSeconds = 30)
        {
            var testTask = testAction();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));

            var completedTask = await Task.WhenAny(testTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                throw new TimeoutException($"测试执行超过 {timeoutSeconds} 秒，已中断");
            }

            // 确保测试任务完成（如果已经完成，这会立即返回）
            await testTask;
        }

        /// <summary>
        /// 运行测试并设置超时，防止测试卡死（带返回值）
        /// </summary>
        public static async Task<T> RunTestWithTimeout<T>(Func<Task<T>> testAction, int timeoutSeconds = 30)
        {
            var testTask = testAction();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));

            var completedTask = await Task.WhenAny(testTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                throw new TimeoutException($"测试执行超过 {timeoutSeconds} 秒，已中断");
            }

            return await testTask;
        }
    }
}