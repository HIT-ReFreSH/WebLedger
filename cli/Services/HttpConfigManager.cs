using System.Net.Http.Json;
using HitReFreSH.WebLedger.Data;
using HitReFreSH.WebLedger.Services;

namespace HitReFreSH.WebLedger.CLI.Services;

public class HttpConfigManager : IConfigManager
{
    private readonly HttpClient _http;

    public HttpConfigManager(HttpClient http)
    {
        _http = http;
    }

    public Task<string> AddAccess(string name)
    {
        return _http.GetStringAsync($"/config/grant?name={name}");
    }

    public async Task RemoveAccess(string name)
    {
        await _http.GetAsync($"/config/cancel?name={name}");
    }

    public async Task<LedgerAccess[]> GetAllAccess()
    {
        return await _http.GetFromJsonAsync<LedgerAccess[]>("/config/access") ?? Array.Empty<LedgerAccess>();
    }
}