#pragma warning disable 8618
using System.ComponentModel.DataAnnotations;

namespace HitReFreSH.WebLedger.Data;

public class LedgerViewTemplate
{
    [Key, MaxLength(64)] public string Name { get; set; }
    [MaxLength(4096)] public string Categories { get; set; }
    public bool IsIncome { get; set; }
}