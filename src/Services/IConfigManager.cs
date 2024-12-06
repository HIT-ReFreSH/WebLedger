using HitRefresh.WebLedger.Data;

namespace HitRefresh.WebLedger.Services;

public interface IConfigManager
{
    public Task<string> AddAccess(string name);
    public Task RemoveAccess(string name);
    public Task<LedgerAccess[]> GetAllAccess();
    public Task<bool> CheckAccess(string name, string key);
    public Task<bool> CheckDuplicate(string name);
}