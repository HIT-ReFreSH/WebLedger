using HitReFreSH.WebLedger.Models;
using HitReFreSH.WebLedger.Services;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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

    [HttpPost("view-automation/add")]
    public async Task<IActionResult> EnableViewAutomation([FromBody] ViewAutomation automation)
    {
        await _ledgerManager.EnableViewAutomation(automation);
        return NoContent();
    }
    [HttpPost("view-automation/remove")]
    public async Task<IActionResult> DisableViewAutomation([FromBody] ViewAutomation automation)
    {
        await _ledgerManager.DisableViewAutomation(automation);
        return NoContent();
    }
    [HttpPut("view-template")]
    public async Task<IActionResult> AddOrUpdateViewTemplate([FromBody] ViewTemplate template)
    {
        await _ledgerManager.AddOrUpdateViewTemplate(template);
        return NoContent();
    }

    [HttpDelete("view-template")]
    public async Task<IActionResult> RemoveViewTemplate([FromQuery] string template)
    {
        await _ledgerManager.RemoveViewTemplate(template);
        return NoContent();
    }

    [HttpPost("view")]
    public async Task<IActionResult> AddView([FromBody] View view)
    {
        await _ledgerManager.AddView(view);
        return NoContent();
    }

    [HttpDelete("view")]
    public async Task<IActionResult> RemoveView([FromQuery] string view)
    {
        await _ledgerManager.RemoveView(view);
        return NoContent();
    }

    [HttpGet("view")]
    public async Task<ActionResult<IList<string>>> GetAllViewNames()
    {
        return Ok(await _ledgerManager.GetAllViewNames());
    }

    [HttpGet("view-templates")]
    public async Task<ActionResult<IList<string>>> GetAllViewTemplateNames()
    {
        return Ok(await _ledgerManager.GetAllViewTemplateNames());
    }
    [HttpGet("view-template/{template}")]
    public async Task<ActionResult<ViewTemplate>> GetViewTemplate([FromRoute]string name)
    {
        return Ok(await _ledgerManager.GetViewTemplate(name));
    }

    [HttpGet("view-automation")]
    public async Task<ActionResult<IList<ViewAutomation>>> GetAllViewAutomation()
    {
        return Ok(await _ledgerManager.GetAllViewAutomation());
    }
    [HttpPost("query")]
    public async Task<ActionResult<ViewQueryResult>> Query([FromBody]ViewQueryOption view)
    {
        return Ok(await _ledgerManager.Query(view));
    }

    [HttpPost("query-graphical")]
    public ActionResult ViewQueryResultPage([FromBody] ViewQueryOption view)
    {
        return RedirectToPage("/"+nameof(ViewQueryResultPage), view);
    }
}