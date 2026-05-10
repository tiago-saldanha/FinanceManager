using FinanceManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinanceManager.Infrastructure.Tenancy;

/// <summary>
/// Cria e inicializa o banco SQLite de um novo usuário no momento do cadastro.
/// </summary>
public class TenantProvisionerService(IConfiguration configuration)
{
    public void ProvisionTenant(string userId)
    {
        var baseFolder = configuration["TenantDb:BaseFolder"]!;
        Directory.CreateDirectory(baseFolder);

        var connectionString = $"Data Source={Path.Combine(baseFolder, $"user_{userId}.db")}";

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlite(connectionString)
            .Options;

        using var context = new TenantDbContext(options);
        context.Database.EnsureCreated();
    }
}
