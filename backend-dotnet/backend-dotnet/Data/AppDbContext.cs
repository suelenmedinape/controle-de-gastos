using backend_dotnet.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend_dotnet.Data;

public class AppDbContext : DbContext
{
    public DbSet<Person> persons { get; set; }
    public DbSet<Categories> categories { get; set; }
    public DbSet<Transaction> transactions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // PERSON -> TRANSACTION (1:N)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Person)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // CATEGORIES -> TRANSACTION (1:N)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Convertendo enum Finance para string no banco de dados
        modelBuilder.Entity<Categories>()
            .Property(c => c.Purpose)
            .HasConversion<string>();
            
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>();
            
        // Índices para melhor performance
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.PersonId);
            
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.CategoryId); 
    }
}