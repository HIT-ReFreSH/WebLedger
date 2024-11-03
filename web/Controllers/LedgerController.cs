using HitRefresh.WebLedger.Models;
using HitRefresh.WebLedger.Services;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace HitRefresh.WebLedger.Web.Controllers;

[Route("[controller]")]
public class LedgerController(ILedgerManager ledgerManager) : Controller
{
    [HttpPost("entry")]
    public async Task<ActionResult<string>> Insert([FromBody] Entry entry)
    {
        try
        {
            var eid = await ledgerManager.Insert(entry);
            return eid;
        }
        catch (TypeUndefinedException e)
        {
            return BadRequest(e);
        }
    }
    [HttpDelete("entry")]
    public async Task<ActionResult<string>> Delete([FromQuery] Guid id)
    {

        await ledgerManager.Remove(id);
        return NoContent();

    }
    [HttpPut("category")]
    public async Task<IActionResult> AddOrUpdateCategory([FromBody]Category category)
    {
        await ledgerManager.AddOrUpdateCategory(category);
        return NoContent();
    }

    [HttpDelete("category")]
    public async Task<IActionResult> RemoveCategory([FromQuery]string category)
    {
        await ledgerManager.RemoveCategory(category);
        return NoContent();
    }

    [HttpPost("select")]
    public async Task<IActionResult> Select([FromBody] SelectOption option)
    {
        return Ok(await ledgerManager.Select(option));
    }

    [HttpGet("category")]
    public async Task<IActionResult> GetAllCategories()
    {
        return Ok(await ledgerManager.GetAllCategories());
    }

    [HttpPost("view-automation/add")]
    public async Task<IActionResult> EnableViewAutomation([FromBody] ViewAutomation automation)
    {
        await ledgerManager.EnableViewAutomation(automation);
        return NoContent();
    }
    [HttpPost("view-automation/remove")]
    public async Task<IActionResult> DisableViewAutomation([FromBody] ViewAutomation automation)
    {
        await ledgerManager.DisableViewAutomation(automation);
        return NoContent();
    }
    [HttpPut("view-template")]
    public async Task<IActionResult> AddOrUpdateViewTemplate([FromBody] ViewTemplate template)
    {
        await ledgerManager.AddOrUpdateViewTemplate(template);
        return NoContent();
    }

    [HttpDelete("view-template")]
    public async Task<IActionResult> RemoveViewTemplate([FromQuery] string template)
    {
        await ledgerManager.RemoveViewTemplate(template);
        return NoContent();
    }

    [HttpPost("view")]
    public async Task<IActionResult> AddView([FromBody] View view)
    {
        await ledgerManager.AddView(view);
        return NoContent();
    }

    [HttpDelete("view")]
    public async Task<IActionResult> RemoveView([FromQuery] string view)
    {
        await ledgerManager.RemoveView(view);
        return NoContent();
    }

    [HttpGet("view")]
    public async Task<ActionResult<IList<string>>> GetAllViewNames()
    {
        return Ok(await ledgerManager.GetAllViewNames());
    }

    [HttpGet("view-templates")]
    public async Task<ActionResult<IList<string>>> GetAllViewTemplateNames()
    {
        return Ok(await ledgerManager.GetAllViewTemplateNames());
    }
    [HttpGet("view-template")]
    public async Task<ActionResult<ViewTemplate>> GetViewTemplate([FromQuery]string name)
    {
        return Ok(await ledgerManager.GetViewTemplate(name));
    }

    [HttpGet("view-automation")]
    public async Task<ActionResult<IList<ViewAutomation>>> GetAllViewAutomation()
    {
        return Ok(await ledgerManager.GetAllViewAutomation());
    }
    [HttpPost("query")]
    public async Task<ActionResult<ViewQueryResult>> Query([FromBody]ViewQueryOption view)
    {
        return Ok(await ledgerManager.Query(view));
    }

    [HttpPost("query-graphical")]
    public ActionResult ViewQueryResultPage([FromBody] ViewQueryOption view)
    {
        return RedirectToPage("/"+nameof(ViewQueryResultPage), view);
    }
}