using System.Net;
using System.Net.Http.Json;
using System.Web;
using HitRefresh.WebLedger.Models;
using HitRefresh.WebLedger.Services;

namespace HitRefresh.WebLedger.CLI.Services;

public class HttpLedgerManager(HttpClient http) : ILedgerManager
{
    public async Task<string> Insert(Entry entry)
    {
        var r = await http.PostAsync("/ledger/entry", JsonContent.Create(entry));
        if (r.StatusCode == HttpStatusCode.BadRequest) throw new TypeUndefinedException(entry.Type);
        return await r.Content.ReadAsStringAsync();
    }

    public async Task Remove(Guid id)
    {
        await http.DeleteAsync($"/ledger/entry?id={id}");
        
    }
    public async Task AddOrUpdateCategory(Category category)
    {
        await http.PutAsJsonAsync("/ledger/category", category);
    }

    public async Task RemoveCategory(string category)
    {
        await http.DeleteAsync($"/ledger/category?category={category}");
    }

    public async Task<IList<RecordedEntry>> Select(SelectOption option)
    {
        return await (await http.PostAsJsonAsync("/ledger/select", option))
            .Content.ReadFromJsonAsync<List<RecordedEntry>>() ?? new List<RecordedEntry>();
    }

    public async Task<IList<Category>> GetAllCategories()
    {
        return await http.GetFromJsonAsync<List<Category>>("/ledger/category") ?? new List<Category>();
    }

    public async Task EnableViewAutomation(ViewAutomation automation)
    {
        await http.PostAsJsonAsync("/ledger/view-automation/add", automation);
    }

    public async Task DisableViewAutomation(ViewAutomation automation)
    {
        await http.PostAsJsonAsync("/ledger/view-automation/remove", automation);
    }

    public async Task AddOrUpdateViewTemplate(ViewTemplate template)
    {
        await http.PutAsJsonAsync("/ledger/view-template", template);
    }

    public async Task RemoveViewTemplate(string template)
    {
        await http.DeleteAsync($"/ledger/view-template?template={HttpUtility.UrlEncode(template)}");

    }

    public async Task AddView(View view)
    {
        await http.PostAsJsonAsync("/ledger/view", view);
    }

    public async Task RemoveView(string view)
    {
        await http.DeleteAsync($"/ledger/view?view={HttpUtility.UrlEncode(view)}");
    }

    public async Task<IList<string>> GetAllViewNames()
    {
        return (await http.GetFromJsonAsync<List<string>>("/ledger/view"))??new();
    }

    public async Task<IList<string>> GetAllViewTemplateNames()
    {
        return (await http.GetFromJsonAsync<List<string>>("/ledger/view-templates")) ?? new();
    }

    public async Task<ViewTemplate> GetViewTemplate(string name)
    {
        return (await http.GetFromJsonAsync<ViewTemplate>($"/ledger/view-template?name={HttpUtility.UrlEncode(name)}"))
            ??throw new ViewTemplateUndefinedException(name);
    }

    public async Task<IList<ViewAutomation>> GetAllViewAutomation()
    {
        return(await http.GetFromJsonAsync<List<ViewAutomation>>("/ledger/view-automation")) ?? new();
    }

    public async Task<ViewQueryResult> Query(ViewQueryOption view)
    {
        var resp = await http.PostAsJsonAsync("/ledger/query", view);
        return await resp.Content.ReadFromJsonAsync<ViewQueryResult>()??throw new Exception(await resp.Content.ReadAsStringAsync());
    }
}