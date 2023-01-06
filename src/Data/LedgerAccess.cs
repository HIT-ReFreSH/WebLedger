#pragma warning disable 8618
using System.ComponentModel.DataAnnotations;

namespace HitReFreSH.WebLedger.Data;

public class LedgerAccess
{
    [Key] [MaxLength(64)] public string Name { get; set; }
    [MaxLength(64)] public string Key { get; set; }
}