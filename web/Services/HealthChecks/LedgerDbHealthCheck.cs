using Microsoft.Extensions.Diagnostics.HealthChecks;
using HitRefresh.WebLedger.Data;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;

namespace HitRefresh.WebLedger.Web.Services.HealthChecks;

public class LedgerDbHealthCheck : IHealthCheck
{
    private readonly LedgerContext _context;

    public LedgerDbHealthCheck(LedgerContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object?>();
        try
        {
            // get underlying connection string
            var connStr = _context.Database.GetDbConnection().ConnectionString ?? string.Empty;
            data["connectionString.masked"] = MaskPassword(connStr);

            // minimal parse of key=value; entries
            var parsed = ParseConnectionString(connStr);
            if (parsed.TryGetValue("server", out var host)) data["host"] = host;
            if (parsed.TryGetValue("port", out var port)) data["port"] = port;
            if (parsed.TryGetValue("database", out var database)) data["database"] = database;
            if (parsed.TryGetValue("user", out var user)) data["user"] = user;

            // check TCP connectivity to host:port
            var hostToTest = parsed.TryGetValue("server", out var hs) ? hs : null;
            var portToTest = parsed.TryGetValue("port", out var ps) && int.TryParse(ps, out var pVal) ? pVal : 3306;
            if (!string.IsNullOrEmpty(hostToTest))
            {
                var reachable = await CheckTcpConnectivity(hostToTest, portToTest, 2000);
                data["hostReachable"] = reachable;
            }

            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            data["canConnect"] = canConnect;
            if (canConnect)
            {
                return HealthCheckResult.Healthy("Database reachable", data);
            }

            // Try to get a helpful message from the DB by performing a simple query
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            }
            catch (Exception exQuery)
            {
                data["lastError"] = exQuery.Message;
            }

            return HealthCheckResult.Unhealthy("Cannot connect to database", data: data);
        }
        catch (Exception ex)
        {
            data["exception"] = ex.Message;
            return HealthCheckResult.Unhealthy("Database check exception: " + ex.Message, ex, data);
        }
    }

    private static async Task<bool> CheckTcpConnectivity(string host, int port, int timeoutMs)
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(host, port);
            var completed = await Task.WhenAny(connectTask, Task.Delay(timeoutMs));
            if (completed != connectTask)
            {
                return false;
            }
            // If task completed, check if connected
            return client.Connected;
        }
        catch
        {
            return false;
        }
    }

    private static Dictionary<string, string> ParseConnectionString(string connStr)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(connStr)) return result;
        var parts = connStr.Split(';');
        foreach (var p in parts)
        {
            var kv = p.Split('=', 2);
            if (kv.Length != 2) continue;
            var key = kv[0].Trim();
            var value = kv[1].Trim();
            result[key] = value;
        }
        return result;
    }

    private static string MaskPassword(string connStr)
    {
        if (string.IsNullOrEmpty(connStr)) return connStr;
        var parts = connStr.Split(';');
        for (int i = 0; i < parts.Length; i++)
        {
            var kv = parts[i].Split('=', 2);
            if (kv.Length == 2 && kv[0].Trim().Equals("password", StringComparison.OrdinalIgnoreCase))
            {
                parts[i] = kv[0] + "=REDACTED";
            }
        }
        return string.Join(";", parts);
    }
}
