using FinanceManager.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Infrastructure.Tests.Data
{
    public class DatabaseFixture
    {
        public TenantDbContext CreateContext()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TenantDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new TenantDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}