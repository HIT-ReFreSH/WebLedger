#pragma warning disable 8618
using System.ComponentModel.DataAnnotations;

namespace HitReFreSH.WebLedger.Data;

/// <summary>
/// Notice: The Automation with larger type index will be Unexecuted if and only if
/// the smaller ones are all Unexecuted, except Weekly ones.
/// </summary>
public enum LedgerViewAutomationType
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Quarterly = 3,
    Yearly = 4
}

public class LedgerViewAutomation
{
    public LedgerViewAutomationType Type { get; set; }
    [MaxLength(64)] public string TemplateName { get; set; }
    public LedgerViewTemplate Template { get; set; }
}