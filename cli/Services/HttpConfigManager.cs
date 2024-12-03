using System.Net.Http.Json;
using HitRefresh.WebLedger.Data;
using HitRefresh.WebLedger.Services;

namespace HitRefresh.WebLedger.CLI.Services;

public class HttpConfigManager(HttpClient http) : IConfigManager
{
    public Task<string> AddAccess(string name)
    {
        return http.GetStringAsync($"/config/grant?name={name}");
    }

    public async Task RemoveAccess(string name)
    {
        await http.GetAsync($"/config/cancel?name={name}");
    }

    public async Task<LedgerAccess[]> GetAllAccess()
    {
        return await http.GetFromJsonAsync<LedgerAccess[]>("/config/access") ?? Array.Empty<LedgerAccess>();
    }

    public Task<bool> CheckAccess(string name, string key)
    {
        return null;
    }

    public Task<bool> CheckDuplicate(string name)
    {
        return null;
    }
}