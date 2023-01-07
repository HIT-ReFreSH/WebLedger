using HitReFreSH.WebLedger.Models;
using Microsoft.Extensions.DependencyInjection;
using PlasticMetal.MobileSuit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace HitReFreSH.WebLedger.CLI.Services;

public class WebGuiHelper
{
    private readonly IIOHub _io;
    private readonly IServiceProvider _sp;

    public WebGuiHelper(IIOHub io, IServiceProvider sp)
    {
        _io = io;
        _sp = sp;
    }

    public async Task CachedQueryGraphical(ViewQueryOption view)
    {
        var http = _sp.GetService<HttpClient>();
        if (http is null)
        {
            await _io.WriteLineAsync($"No Http Client Available.", OutputType.Error);
            return;
        }
        var resp = await http.PostAsJsonAsync("/ledger/query-graphical", view);
        var html = await resp.Content.ReadAsStringAsync();
        var tempFile = Path.GetTempFileName();
        await System.IO.File.WriteAllTextAsync(tempFile, html);
        System.IO.File.Move(tempFile, $"{tempFile}.html");
        Process.Start(new ProcessStartInfo()
        {
            FileName= $"{tempFile}.html",
            UseShellExecute= true,
        });
    }

}