using HitRefresh.WebLedger.Services;
using Microsoft.AspNetCore.Mvc;
using HitRefresh.WebLedger.Web.Models;
using System.Threading.Tasks;

namespace HitRefresh.WebLedger.Web.Controllers;

[Route("[controller]")]
public class ConfigController(IConfigManager configManager) : Controller
{
    // 之前已有的方法保持不变，此处省略重复的详细注释

    [HttpGet("grant")]
    public async Task<IActionResult> AddAccess([FromQuery] string name)
    {
        return Ok(await configManager.AddAccess(name));
    }

    [HttpGet("cancel")]
    public async Task<IActionResult> RemoveAccess([FromQuery] string name)
    {
        await configManager.RemoveAccess(name);
        return NoContent();
    }

    [HttpGet("access")]
    public async Task<IActionResult> GetAllAccess()
    {
        return Ok(await configManager.GetAllAccess());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Key) ||!await configManager.CheckAccess(request.Name, request.Key))
        {
            return Ok("{\"code\":401,\"message\":\"无效的Name或Key\"}");
        }

        return Ok("{\"code\":200,\"message\":\"登录成功\"}");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginRequest request)
    {
        if (await configManager.CheckDuplicate(request.Name))
        {
            return Ok("{\"code\":400,\"message\":\"用户名已存在\"}");
        }
        var key = await configManager.AddAccess(request.Name);
        return Ok("{\"code\":200,\"message\":\"注册成功\",\"key\":\"" + key + "\"}");
    }

    
}