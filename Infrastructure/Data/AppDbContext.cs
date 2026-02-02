using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(builder =>
            {
                builder.HasKey(c => c.Id);
                
                builder.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(60);
                
                builder.Property(c => c.Description)
                    .HasMaxLength(100);

                builder.HasMany(c => c.Transactions)
                    .WithOne(t => t.Category)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transaction>(builder =>
            {
                builder.Property(t => t.Description)
                    .IsRequired()
                    .HasConversion(description => description.Value, value => new Description(value))
                    .HasMaxLength(100);

                builder.OwnsOne(t => t.Dates, dates => 
                {
                    dates.Property(d => d.CreatedAt);
                    dates.Property(d => d.DueDate);
                });

                builder.Property(t => t.Amount)
                    .IsRequired()
                    .HasConversion(money => money.Value, value => new Money(value))
                    .HasPrecision(18, 2);
            });
        }
    }
}
