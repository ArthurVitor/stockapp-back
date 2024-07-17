using Microsoft.EntityFrameworkCore;
using StockApp.Models.Models;

namespace StockApp.API.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Product> Products { get; set; }
    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<EntryNote> EntryNotes { get; set; }
    public DbSet<ExitNote> ExitNotes { get; set; }
    public DbSet<Batch> Batches { get; set; }
    public DbSet<ProductSubCategory> ProductSubCategories { get; set; }
    public DbSet<ExitNoteBatch> ExitNoteBatches { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Parameters> Parameters { get; set; }
    public DbSet<Transactions> Transactions { get; set; }
    public DbSet<Override> Overrides { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductSubCategory>().HasKey(cs => new { cs.ProductId, cs.SubCategoryId });

        modelBuilder.Entity<ProductSubCategory>()
            .HasOne(cs => cs.Product)
            .WithMany(s => s.ProductSubCategories)
            .HasForeignKey(cs => cs.ProductId);

        modelBuilder.Entity<ProductSubCategory>()
            .HasOne(cs => cs.SubCategory)
            .WithMany(c => c.ProductSubCategories)
            .HasForeignKey(cs => cs.SubCategoryId);

        modelBuilder.Entity<ExitNoteBatch>().HasKey(eb => new { eb.ExitNoteId, eb.BatchId });

        modelBuilder.Entity<ExitNoteBatch>()
            .HasOne(eb => eb.ExitNote)
            .WithMany(en => en.ExitNoteBatches)
            .HasForeignKey(eb => eb.ExitNoteId);

        modelBuilder.Entity<ExitNoteBatch>()
            .HasOne(eb => eb.Batch)
            .WithMany(b => b.ExitNoteBatches)
            .HasForeignKey(eb => eb.BatchId);
    }
}
