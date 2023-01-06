using HitReFreSH.WebLedger.Data;

namespace HitReFreSH.WebLedger.Services;

public interface IConfigManager
{
    public Task<string> AddAccess(string name);
    public Task RemoveAccess(string name);
    public Task<LedgerAccess[]> GetAllAccess();
}