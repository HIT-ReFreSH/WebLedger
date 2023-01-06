using System.Net;
using System.Net.Http.Json;
using HitReFreSH.WebLedger.Models;
using HitReFreSH.WebLedger.Services;

namespace HitReFreSH.WebLedger.CLI.Services;

public class HttpLedgerManager : ILedgerManager
{
    private readonly HttpClient _http;

    public HttpLedgerManager(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> Insert(Entry entry)
    {
        var r = await _http.PostAsync("/ledger/entry", JsonContent.Create(entry));
        if (r.StatusCode == HttpStatusCode.BadRequest) throw new TypeUndefinedException(entry.Type);
        return await r.Content.ReadAsStringAsync();
    }

    public async Task AddOrUpdateCategory(Category category)
    {
        await _http.PutAsJsonAsync("/ledger/category", category);
    }

    public async Task RemoveCategory(string category)
    {
        await _http.DeleteAsync($"/ledger/category?category={category}");
    }

    public async Task<IList<Entry>> Select(SelectOption option)
    {
        return await (await _http.PostAsJsonAsync("/ledger/select", option))
            .Content.ReadFromJsonAsync<List<Entry>>() ?? new List<Entry>();
    }

    public async Task<IList<Category>> GetAllCategories()
    {
        return await _http.GetFromJsonAsync<List<Category>>("/ledger/category") ?? new List<Category>();
    }
}