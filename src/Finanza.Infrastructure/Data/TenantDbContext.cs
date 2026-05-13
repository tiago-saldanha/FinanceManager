using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Data;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category>    Categories   { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(60);

            builder.Property(c => c.Description)
                .HasMaxLength(100);

            builder.HasMany(c => c.Transactions)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<Transaction>(builder =>
        {
            builder.Property(t => t.Description)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(100);

            builder.ComplexProperty(t => t.Dates, dates => dates.Property(d => d.CreatedAt));
            builder.ComplexProperty(t => t.Dates, dates => dates.Property(d => d.DueDate));

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);
        });
    }
}
