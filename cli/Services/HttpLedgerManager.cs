using System.Net;
using System.Net.Http.Json;
using System.Web;
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

    public async Task EnableViewAutomation(ViewAutomation automation)
    {
        await _http.PostAsJsonAsync("/ledger/view-automation/add", automation);
    }

    public async Task DisableViewAutomation(ViewAutomation automation)
    {
        await _http.PostAsJsonAsync("/ledger/view-automation/remove", automation);
    }

    public async Task AddOrUpdateViewTemplate(ViewTemplate template)
    {
        await _http.PutAsJsonAsync("/ledger/view-template", template);
    }

    public async Task RemoveViewTemplate(string template)
    {
        await _http.DeleteAsync($"/ledger/view-template?template={HttpUtility.UrlEncode(template)}");

    }

    public async Task AddView(View view)
    {
        await _http.PostAsJsonAsync("/ledger/view", view);
    }

    public async Task RemoveView(string view)
    {
        await _http.DeleteAsync($"/ledger/view?view={HttpUtility.UrlEncode(view)}");
    }

    public async Task<IList<string>> GetAllViewNames()
    {
        return (await _http.GetFromJsonAsync<List<string>>("/ledger/view"))??new();
    }

    public async Task<IList<string>> GetAllViewTemplateNames()
    {
        return (await _http.GetFromJsonAsync<List<string>>("/ledger/view-templates")) ?? new();
    }

    public async Task<ViewTemplate> GetViewTemplate(string name)
    {
        return (await _http.GetFromJsonAsync<ViewTemplate>($"/ledger/view-template/{HttpUtility.UrlEncode(name)}"))
            ??throw new ViewTemplateUndefinedException(name);
    }

    public async Task<IList<ViewAutomation>> GetAllViewAutomation()
    {
        return(await _http.GetFromJsonAsync<List<ViewAutomation>>("/ledger/view-automation")) ?? new();
    }

    public async Task<ViewQueryResult> Query(ViewQueryOption view)
    {
        var resp = await _http.PostAsJsonAsync("/ledger/query", view);
        return await resp.Content.ReadFromJsonAsync<ViewQueryResult>()??throw new Exception(await resp.Content.ReadAsStringAsync());
    }
}