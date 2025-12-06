using HitRefresh.WebLedger.Data;
using HitRefresh.WebLedger.Services;
using HitRefresh.WebLedger.Web.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Steeltoe.Extensions.Configuration.Placeholder;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HitRefresh.WebLedger.Web
{
    // ========== 静态辅助方法 ==========
    public static class HealthCheckResponseWriters
    {
        public static async Task WriteBasicHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                duration = $"{report.TotalDuration.TotalMilliseconds}ms",
                environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>().EnvironmentName,
                version = GetApplicationVersion(),
                checks = report.Entries.Count,
                healthy = report.Entries.Count(e => e.Value.Status == HealthStatus.Healthy),
                degraded = report.Entries.Count(e => e.Value.Status == HealthStatus.Degraded),
                unhealthy = report.Entries.Count(e => e.Value.Status == HealthStatus.Unhealthy)
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        public static async Task WriteDetailedHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var options = new JsonSerializerOptions
            {
                WriteIndented = context.Request.Query.ContainsKey("pretty"),
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                totalDuration = $"{report.TotalDuration.TotalMilliseconds}ms",
                environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>().EnvironmentName,
                version = GetApplicationVersion(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    duration = $"{entry.Value.Duration.TotalMilliseconds}ms",
                    tags = entry.Value.Tags,
                    exception = entry.Value.Exception?.Message,
                    data = entry.Value.Data
                }),
                summary = new
                {
                    healthy = report.Entries.Count(e => e.Value.Status == HealthStatus.Healthy),
                    degraded = report.Entries.Count(e => e.Value.Status == HealthStatus.Degraded),
                    unhealthy = report.Entries.Count(e => e.Value.Status == HealthStatus.Unhealthy),
                    total = report.Entries.Count
                }
            };

            await context.Response.WriteAsJsonAsync(response, options);
        }

        private static string GetApplicationVersion()
        {
            try
            {
                var assembly = typeof(Program).Assembly;
                var version = assembly.GetName().Version;
                var informationalVersion = assembly
                    .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
                    .Cast<System.Reflection.AssemblyInformationalVersionAttribute>()
                    .FirstOrDefault()?.InformationalVersion;

                return informationalVersion ?? version?.ToString() ?? "1.0.0";
            }
            catch
            {
                return "unknown";
            }
        }
    }

    // ========== 健康检查类 ==========
    public class DatabaseConnectionHealthCheck : IHealthCheck
    {
        private readonly LedgerContext _dbContext;
        private readonly IConfiguration _configuration;

        public DatabaseConnectionHealthCheck(LedgerContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            //return HealthCheckResult.Unhealthy("TEST: Forced failure for testing");
            try
            {
                var connectionString = _configuration["ConnectionStrings:mysql"];
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    return HealthCheckResult.Unhealthy(
                        description: "Database connection string is not configured",
                        exception: null,
                        data: new Dictionary<string, object>
                        {
                            ["error"] = "Missing ConnectionStrings:mysql in configuration"
                        });
                }

                var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
                
                if (!canConnect)
                {
                    return HealthCheckResult.Unhealthy(
                        description: "Cannot connect to MySQL database",
                        exception: null,
                        data: new Dictionary<string, object>
                        {
                            ["connection_string"] = MaskConnectionString(connectionString),
                            ["error"] = "Database connection failed"
                        });
                }

                try
                {
                    var testQuery = await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
                    
                    // Healthy 方法没有 exception 参数！
                    return HealthCheckResult.Healthy(
                        description: "Database connection is healthy",
                        data: new Dictionary<string, object>
                        {
                            ["provider"] = "MySQL",
                            ["test_query_result"] = testQuery,
                            ["connection_info"] = GetConnectionInfo(connectionString)
                        });
                }
                catch (Exception queryEx)
                {
                    return HealthCheckResult.Unhealthy(
                        description: "Database query failed",
                        exception: queryEx,
                        data: new Dictionary<string, object>
                        {
                            ["connection_info"] = GetConnectionInfo(connectionString),
                            ["error_type"] = queryEx.GetType().Name
                        });
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    description: "Database health check failed unexpectedly",
                    exception: ex,
                    data: new Dictionary<string, object>
                    {
                        ["error_type"] = ex.GetType().Name,
                        ["error_message"] = ex.Message
                    });
            }
        }

        private static string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return "Connection string is empty";
                
            try
            {
                var parts = connectionString.Split(';');
                var maskedParts = parts.Select(part =>
                {
                    if (part.Trim().StartsWith("password=", StringComparison.OrdinalIgnoreCase) ||
                        part.Trim().StartsWith("pwd=", StringComparison.OrdinalIgnoreCase))
                    {
                        return "password=***";
                    }
                    return part;
                });
                return string.Join(';', maskedParts);
            }
            catch
            {
                return "***";
            }
        }

        private static Dictionary<string, string> GetConnectionInfo(string connectionString)
        {
            var info = new Dictionary<string, string>();
            
            try
            {
                var parts = connectionString.Split(';');
                
                foreach (var part in parts)
                {
                    var trimmedPart = part.Trim();
                    if (trimmedPart.Contains('='))
                    {
                        var keyValue = trimmedPart.Split('=', 2);
                        var key = keyValue[0].ToLower();
                        var value = keyValue[1];
                        
                        if (key == "server" || key == "database" || key == "user" || key == "port")
                        {
                            info[key] = value;
                        }
                    }
                }
            }
            catch
            {
                // 忽略解析错误
            }
            
            return info;
        }
    }

    public class MemoryHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var allocated = GC.GetTotalMemory(forceFullCollection: false);
                var memoryInfo = GC.GetGCMemoryInfo();
                var totalMemory = memoryInfo.TotalAvailableMemoryBytes;
                var memoryUsagePercent = totalMemory > 0 ? (double)allocated / totalMemory * 100 : 0;

                var data = new Dictionary<string, object>
                {
                    ["allocated_bytes"] = allocated,
                    ["allocated_mb"] = Math.Round(allocated / 1024.0 / 1024.0, 2),
                    ["total_available_bytes"] = totalMemory,
                    ["total_available_mb"] = Math.Round(totalMemory / 1024.0 / 1024.0, 2),
                    ["memory_usage_percent"] = Math.Round(memoryUsagePercent, 2),
                    ["gen0_collections"] = GC.CollectionCount(0),
                    ["gen1_collections"] = GC.CollectionCount(1),
                    ["gen2_collections"] = GC.CollectionCount(2),
                    ["gc_mode"] = GC.MaxGeneration == 2 ? "Workstation" : "Server",
                    ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                const double warningThreshold = 80.0;
                const double criticalThreshold = 90.0;

                if (memoryUsagePercent < warningThreshold)
                {
                    // Healthy 方法没有 exception 参数！
                    return Task.FromResult(HealthCheckResult.Healthy(
                        description: $"Memory usage normal: {Math.Round(memoryUsagePercent, 1)}%",
                        data: data));
                }
                else if (memoryUsagePercent < criticalThreshold)
                {
                    // Degraded 方法没有 exception 参数！
                    return Task.FromResult(HealthCheckResult.Degraded(
                        description: $"Memory usage high: {Math.Round(memoryUsagePercent, 1)}%",
                        data: data));
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(
                        description: $"Memory usage critical: {Math.Round(memoryUsagePercent, 1)}%",
                        exception: null,
                        data: data));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    description: "Failed to check memory health",
                    exception: ex,
                    data: new Dictionary<string, object>
                    {
                        ["error_type"] = ex.GetType().Name,
                        ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }));
            }
        }
    }

    // ========== 主程序类 ==========
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddPlaceholderResolver();

            var mysql = builder.Configuration["ConnectionStrings:mysql"];

            // ========== 添加健康检查服务 ==========
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<LedgerContext>(
                    name: "mysql-database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "database", "ready", "critical" })
                .AddCheck<DatabaseConnectionHealthCheck>(
                    "database-connection",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "database", "ready" }) 
                .AddCheck<MemoryHealthCheck>(
                    "memory-usage",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "system", "ready" })   
                .AddCheck("self", () => 
                    HealthCheckResult.Healthy(
                        description: "Application is running",
                        data: null));

            // 注册自定义健康检查服务
builder.Services.AddScoped<DatabaseConnectionHealthCheck>();
builder.Services.AddScoped<MemoryHealthCheck>();
            // ========== 原有的服务配置 ==========
            builder.Services.AddScoped<AccessMiddleware>();
            builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddScoped<IConfigManager, DirectConfigManager>();
            builder.Services.AddScoped<ILedgerManager, DirectLedgerManager>();
            builder.Services.AddLogging();

            builder.Services.AddSingleton<AccessMiddleware>();
            builder.Services.AddDbContext<LedgerContext>(
                c => c.UseMySql(
                    mysql,
                    ServerVersion.AutoDetect(mysql),
                    b => b.MigrationsAssembly("HitRefresh.WebLedger")));
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // ========== 健康检查端点配置 ==========
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready"),
                ResponseWriter = HealthCheckResponseWriters.WriteBasicHealthCheckResponse,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                AllowCachingResponses = false
            });

            app.MapHealthChecks("/health/detailed", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = HealthCheckResponseWriters.WriteDetailedHealthCheckResponse,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });

            // Liveness 检查
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("critical"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = report.Status.ToString(),
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        checks = report.Entries.Count
                    });
                }
            });

            // ========== 原有的管道配置 ==========
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseMiddleware<ErrorHandlingMiddleware>();
            }
            else
            {
                app.UseHsts();
                /*using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<LedgerContext>();
                    context.Database.Migrate();
                }*/
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseAuthorization();
            app.UseMiddleware<AccessMiddleware>();
            app.MapRazorPages();
            app.MapControllers();

            app.Run();
        }
    }
}