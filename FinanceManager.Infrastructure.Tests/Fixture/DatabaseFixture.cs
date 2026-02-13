using FinanceManager.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Infrastructure.Tests.Data
{
    public class DatabaseFixture : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _options;

        public DatabaseFixture()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = new AppDbContext(_options);
            context.Database.EnsureCreated();
        }

        public AppDbContext CreateContext()
            => new AppDbContext(_options);

        public void Dispose()
            => _connection.Dispose();
    }
}