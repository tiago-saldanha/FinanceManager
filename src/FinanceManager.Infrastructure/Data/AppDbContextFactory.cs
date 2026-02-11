using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FinanceManager.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            builder.UseSqlite(
                "Data Source=financeiro.db",
                b => b.MigrationsAssembly("Infrastructure")
            );

            return new AppDbContext(builder.Options);
        }
    }
}
