using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HitRefresh.WebLedger.Data;

public class LedgerEntry
{
    [Key] public Guid Id { get; set; }
    [Required] public bool IsIncome { get; set; }

    public DateTime CreateTime { get; set; }

    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    //public DateTime UpdateTime { get; set; }
    public DateTime GivenTime { get; set; }

    /// <summary>
    ///     Amount of Money spent or got, with sign (income with + and outcome with -).
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [MaxLength(256)] public string Description { get; set; }
    public LedgerEntryType Type { get; set; }
    public string TypeName { get; set; }
    public LedgerEntryCategory Category { get; set; }
    public string CategoryName { get; set; }
}