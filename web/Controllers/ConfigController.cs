using HitReFreSH.WebLedger.Services;
using Microsoft.AspNetCore.Mvc;

namespace HitReFreSH.WebLedger.Web.Controllers;

[Route("[controller]")]
public class ConfigController : Controller
{
    private readonly IConfigManager _configManager;

    public ConfigController(IConfigManager configManager)
    {
        _configManager = configManager;
    }

    [HttpGet("grant")]
    public async Task<IActionResult> AddAccess([FromQuery] string name)
    {
        return Ok(await _configManager.AddAccess(name));
    }

    [HttpGet("cancel")]
    public async Task<IActionResult> RemoveAccess([FromQuery] string name)
    {
        await _configManager.RemoveAccess(name);
        return NoContent();
    }

    [HttpGet("access")]
    public async Task<IActionResult> GetAllAccess()
    {
        return Ok(await _configManager.GetAllAccess());
    }
}