using HitReFreSH.WebLedger.Models;
using Microsoft.EntityFrameworkCore;

namespace HitReFreSH.WebLedger.Data;

public class LedgerContext : DbContext
{
    public LedgerContext(DbContextOptions<LedgerContext> options) : base(options)
    {
    }

    public DbSet<LedgerEntry> LedgerEntries { get; set; }
    public DbSet<LedgerEntryCategory> Categories { get; set; }
    public DbSet<LedgerEntryType> Types { get; set; }
    public DbSet<LedgerConfig> Configs { get; set; }
    public DbSet<LedgerAccess> Access { get; set; }
    public DbSet<LedgerView> Views { get; set; }
    public DbSet<LedgerViewTemplate> ViewTemplates { get; set; }
    public DbSet<LedgerViewAutomation> ViewAutomation { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<LedgerViewAutomation>()
            .HasKey(a => new { a.TemplateName, a.Type });

    }
}