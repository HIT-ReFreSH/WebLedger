using HitRefresh.WebLedger.Services;
using Microsoft.AspNetCore.Mvc;

namespace HitRefresh.WebLedger.Web.Controllers;

[Route("[controller]")]
public class ConfigController(IConfigManager configManager) : Controller
{
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
}