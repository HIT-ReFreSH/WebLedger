#pragma warning disable 8618

using System.ComponentModel.DataAnnotations;

namespace HitReFreSH.WebLedger.Data;

/// <summary>
/// 
/// </summary>
public class LedgerView
{
    [Key, MaxLength(64)] public string Name { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [MaxLength(64)] public string TemplateName { get; set; }
    public LedgerViewTemplate Template { get; set; }
}