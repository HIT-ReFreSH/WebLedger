using HitRefresh.WebLedger.Models;
using HitRefresh.WebLedger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HitRefresh.WebLedger.Web.Pages;

public record ChartJsDataSet(string Label, List<decimal> Data, List<string> BackgroundColor, List<string> BorderColor,
    decimal BorderWidth);

public record ChartJsData(List<string> Labels, List<ChartJsDataSet> Datasets);
public class ViewQueryResultPageModel : PageModel
{
    private readonly ILedgerManager _ledger;
    public ViewQueryResult? Result{ get; private set; }
    public List<Entry> Raw { get; private set; } = new();
    public List<(string,decimal,decimal)> ByCat { get; private set; } = new();
    public List<(string, decimal, decimal)> ByTime { get; private set; } = new();
    public ChartJsData ByCatChartJs { get; private set; }
    public ChartJsData ByTimeChartJs { get; private set; }
    public decimal Total { get; private set; }
    public ViewQueryResultPageModel(ILedgerManager ledger)
    {
        _ledger = ledger;
    }

    private static readonly string[] BackgroundColors =
    {
        "rgba(255, 99, 132, 0.2)",
        "rgba(54, 162, 235, 0.2)",
        "rgba(255, 206, 86, 0.2)",
        "rgba(75, 192, 192, 0.2)",
        "rgba(153, 102, 255, 0.2)",
        "rgba(255, 159, 64, 0.2)"
    };
    private static readonly string[] BorderColors =
    {
        "rgba(255,99,132,1)",
        "rgba(54, 162, 235, 1)",
        "rgba(255, 206, 86, 1)",
        "rgba(75, 192, 192, 1)",
        "rgba(153, 102, 255, 1)",
        "rgba(255, 159, 64, 1)"
    };

    private static ChartJsData TupleListToChartJsData(string title,List<(string, decimal, decimal)> tupleList)
    {
        var labels = new List<string>();
        var values = new List<decimal>();
        var bgColors = new List<string>();
        var bdColors = new List<string>();
        var length=BackgroundColors.Length;
        foreach (var ((label,data,_),i) in tupleList.Select((d,i)=>(d,i)))
        {
            labels.Add(label);
            values.Add(data);
            bgColors.Add(BackgroundColors[i%length]);
            bdColors.Add(BorderColors[i%length]);
        }

        return new(labels, new() { new(title, values, bgColors, bdColors, 1) });
    }
    public async Task OnGetAsync(string viewName, int limit)
    {
        ViewData["Title"]=viewName;

        Result = await _ledger.Query(new(viewName, limit));
        var (raw,cat,time)=Result;
        Raw=raw;
        Total = cat["(Total)"];
        cat.Remove("(Total)");
        ByCat=cat.Select(x=>(x.Key,x.Value,100*x.Value/Total))
            .OrderByDescending(x=>x.Item3)
            .ToList();
        ByTime=time.Select(x => (x.Key, x.Value, 100 * x.Value / Total))
            .OrderByDescending(x => x.Item1)
            .ToList();

        ByCatChartJs = TupleListToChartJsData("Figure of Categories", ByCat);
        ByTimeChartJs=TupleListToChartJsData("Figure of Times", ByTime);
    }
}