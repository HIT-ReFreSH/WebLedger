#pragma warning disable 8618
using System.ComponentModel.DataAnnotations;

namespace HitRefresh.WebLedger.Data;

public class LedgerConfig
{
    [MaxLength(256)] public string Value { get; set; }
    [Key] [MaxLength(64)] public string Key { get; set; }
}