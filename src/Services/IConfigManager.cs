using HitRefresh.WebLedger.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HitRefresh.WebLedger.Services;

// IConfigManager接口定义，添加了与用户管理相关的新方法声明
public interface IConfigManager
{
    // 之前已有的方法保持不变，此处省略重复的详细注释
    public Task<string> AddAccess(string name);
    public Task RemoveAccess(string name);
    public Task<LedgerAccess[]> GetAllAccess();
    public Task<bool> CheckAccess(string name, string key);
    public Task<bool> CheckDuplicate(string name);

}