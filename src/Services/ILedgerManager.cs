using HitReFreSH.WebLedger.Models;

namespace HitReFreSH.WebLedger.Services;

public interface ILedgerManager
{
    public Task<string> Insert(Entry entry);
    public Task AddOrUpdateCategory(Category category);
    public Task RemoveCategory(string category);
    public Task<IList<Entry>> Select(SelectOption option);
    public Task<IList<Category>> GetAllCategories();
    public Task EnableViewAutomation(ViewAutomation automation);
    public Task DisableViewAutomation(ViewAutomation automation);
    public Task AddOrUpdateViewTemplate(ViewTemplate template);
    public Task RemoveViewTemplate(string template);
    public Task AddView(View view);
    public Task RemoveView(string view);
    public Task<IList<View>> GetAllViews();
    public Task<IList<ViewTemplate>> GetAllViewTemplates();
    public Task<IList<ViewAutomation>> GetAllViewAutomation();
    public Task<ViewQueryResult> Query(ViewQueryOption view);
}