using System.ComponentModel.DataAnnotations;

namespace HitRefresh.WebLedger.Data;

public class LedgerEntryCategory
{
    [Key] [MaxLength(64)] public string Name { get; set; }
    [MaxLength(64)] public string? SuperCategoryName { get; set; }
    public LedgerEntryCategory? SuperCategory { get; set; }
}