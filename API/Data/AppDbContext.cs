using API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
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
                builder.Property(c => c.Name).IsRequired().HasMaxLength(60);
                builder.Property(c => c.Description).HasMaxLength(100);

                builder.HasMany(c => c.Transactions)
                       .WithOne(t => t.Category)
                       .HasForeignKey(t => t.CategoryId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transaction>(builder =>
            {
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Description).IsRequired().HasMaxLength(200);
                builder.Property(t => t.Amount).HasPrecision(18, 2);
            });
        }
    }
}
