using HitReFreSH.WebLedger.Models;
using HitReFreSH.WebLedger.Services;
using Microsoft.AspNetCore.Mvc;

namespace HitReFreSH.WebLedger.Web.Controllers;

[Route("[controller]")]
public class LedgerController : Controller
{
    private readonly ILedgerManager _ledgerManager;

    public LedgerController(ILedgerManager ledgerManager)
    {
        _ledgerManager = ledgerManager;
    }

    [HttpPost("entry")]
    public async Task<ActionResult<string>> Insert([FromBody] Entry entry)
    {
        try
        {
            var eid = await _ledgerManager.Insert(entry);
            return eid;
        }
        catch (TypeUndefinedException e)
        {
            return BadRequest(e);
        }
    }

    [HttpPut("category")]
    public async Task<IActionResult> AddOrUpdateCategory([FromBody]Category category)
    {
        await _ledgerManager.AddOrUpdateCategory(category);
        return NoContent();
    }

    [HttpDelete("category")]
    public async Task<IActionResult> RemoveCategory([FromQuery]string category)
    {
        await _ledgerManager.RemoveCategory(category);
        return NoContent();
    }

    [HttpPost("select")]
    public async Task<IActionResult> Select([FromBody] SelectOption option)
    {
        return Ok(await _ledgerManager.Select(option));
    }

    [HttpGet("category")]
    public async Task<IActionResult> GetAllCategories()
    {
        return Ok(await _ledgerManager.GetAllCategories());
    }
}