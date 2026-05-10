using FinanceManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Infrastructure.Data;

/// <summary>
/// Banco compartilhado — contém apenas as tabelas do ASP.NET Core Identity.
/// Os dados financeiros de cada usuário ficam em seu próprio TenantDbContext.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => base.OnModelCreating(modelBuilder);
}
