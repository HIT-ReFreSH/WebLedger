using System.ComponentModel.DataAnnotations;

namespace HitRefresh.WebLedger.Data;

public class LedgerEntryType
{
    [Key] [MaxLength(64)] public string Name { get; set; }
    [Required] [MaxLength(64)] public string DefaultCategoryName { get; set; }
    public LedgerEntryCategory DefaultCategory { get; set; }
    [Required] public bool DefaultIsIncome { get; set; }
}